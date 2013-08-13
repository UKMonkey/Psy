using System;
using Psy.Core;
using Psy.Core.EpicModel;
using Psy.Graphics.Effects;
using Psy.Graphics.VertexDeclarations;
using SlimMath;

namespace Psy.Graphics.Models.Renderers
{
    public class EpicModelRenderer : IDisposable
    {
        private readonly GraphicsContext _graphicsContext;
        private readonly IVertexRenderer<TexturedColouredVertex4> _vertexRenderer;
        private IEffect _effect;
        public Matrix WorldMatrix;
        private Matrix _viewMat;
        private Matrix _projection;
        private MaterialCache _materials;
        private readonly VertexCuboid _anchorCuboid;
        private int _floorSize;
        public bool Wireframe
        {
            get { return _graphicsContext.FillMode == FillMode.Wireframe; } 
            set { _graphicsContext.FillMode = value ? FillMode.Wireframe : FillMode.Solid; }
        }
        public bool RenderAnchors { get; set; }
        public Anchor SelectedAnchor { get; set; }

        public EpicModelRenderer(GraphicsContext graphicsContext)
        {
            _graphicsContext = graphicsContext;
            _vertexRenderer = _graphicsContext.CreateVertexRenderer<TexturedColouredVertex4>(256);
            _anchorCuboid = new VertexCuboid(new Vector3(0.1f));
        }

        public void Render(EpicModel epicModel)
        {
            foreach (var modelPart in epicModel.ModelParts)
            {
                if (!modelPart.IsARootModelPart)
                    continue;

                ResetWorldMatrix();

                Render(modelPart);
            }
        }

        public void Render(ModelPart modelPart)
        {
            var renderArgs = new RenderArgs(modelPart);

            modelPart.SetRenderArgsSelection(renderArgs);

            WorldMatrix = modelPart.GetAnchorRotationAndPositionMatrix() * WorldMatrix;

            ModelPart(modelPart, renderArgs, false);

            var wireframeMode = Wireframe;

            if (RenderAnchors)
            {
                Wireframe = true;

                foreach (var anchor in modelPart.Anchors)
                {
                    Anchor(anchor);
                }

                Wireframe = wireframeMode;
            }

            var bMat = WorldMatrix;

            foreach (var anchor in modelPart.Anchors)
            {
                if (!anchor.HasChildren)
                    continue;

                WorldMatrix = Matrix.Translation(anchor.Position) * WorldMatrix;
                var wMat = WorldMatrix;

                foreach (var child in anchor.Children)
                {
                    WorldMatrix = Matrix.Translation(child.Position) * WorldMatrix;

                    Render(child);
                    WorldMatrix = wMat;
                }
            }

            WorldMatrix = bMat;
        }


        public void Dispose()
        {
            _vertexRenderer.Dispose();
        }

        public void SetEffect(IEffect effect)
        {
            _effect = effect;
        }

        public void SetMaterials(MaterialCache materials)
        {
            _materials = materials;
        }

        public void ModelPart(ModelPart modelPart, RenderArgs renderArgs, bool wireframe)
        {
            var dataStream = _vertexRenderer.LockVertexBuffer();

            if (!wireframe)
            {
                SetTexture(modelPart.MaterialId);    
            }
            else
            {
                SetTexture(-1);
            }

            var triCount = 0;

            int faceIndex = 0;
            foreach (var modelPartFace in modelPart.Faces)
            {
                var faceArgs = renderArgs.RenderArgsFaces[faceIndex];

                var colour = faceArgs.Colour;

                if (wireframe)
                {
                    colour = Colours.White;
                }
                
                faceIndex++;

                foreach (var modelTriangle in modelPartFace.Triangles)
                {
                    dataStream.Write(new TexturedColouredVertex4
                    {
                        Colour = colour, 
                        Position = new Vector4(modelTriangle.V1, 1.0f), 
                        TextureCoordinate = modelPartFace.TextureCoordinates[modelTriangle.V1TexCoordIndex]
                    });

                    dataStream.Write(new TexturedColouredVertex4
                    {
                        Colour = colour, 
                        Position = new Vector4(modelTriangle.V2, 1.0f), 
                        TextureCoordinate = modelPartFace.TextureCoordinates[modelTriangle.V2TexCoordIndex]
                    });

                    dataStream.Write(new TexturedColouredVertex4
                    {
                        Colour = colour, 
                        Position = new Vector4(modelTriangle.V3, 1.0f), 
                        TextureCoordinate = modelPartFace.TextureCoordinates[modelTriangle.V3TexCoordIndex]
                    });
                    triCount++;
                }
            }

            _vertexRenderer.UnlockVertexBuffer();

            _effect.SetMatrix("worldMat", WorldMatrix);
            _effect.SetMatrix("worldViewProjMat", WorldMatrix * _viewMat * _projection);
            _effect.CommitChanges();

            _vertexRenderer.RenderForShader(PrimitiveType.TriangleList, 0, triCount);
        }

        /// <summary>
        /// Reset world matrix, if not specified Identity matrix is used.
        /// </summary>
        /// <param name="matrix"></param>
        public void ResetWorldMatrix(Matrix? matrix = null)
        {
            WorldMatrix = matrix.HasValue ? matrix.Value : Matrix.Identity;
        }

        private void SetTexture(int materialId)
        {
            if (_materials == null || !_materials.HasMaterial(materialId))
            {
                _effect.SetTexture("tex0", _graphicsContext.BlankTexture);
            }
            else
            {
                var material = _materials[materialId];
                _effect.SetTexture("tex0", _graphicsContext.GetTexture(material.TextureName));
            }
        }

        private void SetTexture(TextureAreaHolder textureArea)
        {
            _effect.SetTexture("tex0", textureArea);
        }

        public void SetMatrices(Matrix viewMat, Matrix projection)
        {
            _viewMat = viewMat;
            _projection = projection;
        }

        public void Anchor(Anchor anchor)
        {
            var dataStream = _vertexRenderer.LockVertexBuffer();

            var colour = anchor == SelectedAnchor ? Colours.Yellow : Colours.Green;

            _anchorCuboid.Position = new Vector3();

            int renderCount = 12;

            // front clockwise
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftFrontTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightFrontTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightFrontTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightFrontBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightFrontBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftFrontBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftFrontBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftFrontTop, colour));

            // rear clockwise
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftBackTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightBackTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightBackTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightBackBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightBackBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftBackBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftBackBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftBackTop, colour));

            // front to back
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftFrontTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftBackTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightFrontTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightBackTop, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftFrontBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.LeftBackBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightFrontBottom, colour));
            dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightBackBottom, colour));

            // spike.

            if (anchor.Rotation.Length > 0)
            {

                var pos = _anchorCuboid.RightFrontTop.Translate(0.2f, 0, 0);
                pos = pos.Translate(0, _anchorCuboid.Size.Y / 2.0f, _anchorCuboid.Size.Z);

                dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightFrontTop, colour));
                dataStream.Write(new TexturedColouredVertex4(pos, colour));

                dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightBackTop, colour));
                dataStream.Write(new TexturedColouredVertex4(pos, colour));

                dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightBackBottom, colour));
                dataStream.Write(new TexturedColouredVertex4(pos, colour));

                dataStream.Write(new TexturedColouredVertex4(_anchorCuboid.RightFrontBottom, colour));
                dataStream.Write(new TexturedColouredVertex4(pos, colour));

                renderCount += 4;
            }

            _vertexRenderer.UnlockVertexBuffer();

            SetTexture(-1);

            var posMat = Matrix.Translation(anchor.Position);

            var tmat = WorldMatrix;
            WorldMatrix = anchor.GetRotationMatrix() * posMat * WorldMatrix;

            _effect.SetMatrix("worldMat", WorldMatrix);
            _effect.SetMatrix("worldViewProjMat", WorldMatrix * _viewMat * _projection);

            _effect.CommitChanges();

            WorldMatrix = tmat;

            _vertexRenderer.RenderForShader(PrimitiveType.LineList, 0, renderCount);

            // render child anchor lines.
            var wmat = WorldMatrix;
            foreach (var child in anchor.Children)
            {
                var dataStream2 = _vertexRenderer.LockVertexBuffer();
                dataStream2.Write(new TexturedColouredVertex4(new Vector3(), colour));
                dataStream2.Write(new TexturedColouredVertex4(child.Position, colour));

                _vertexRenderer.UnlockVertexBuffer();

                _vertexRenderer.RenderForShader(PrimitiveType.LineList, 0, 1);
            }

            WorldMatrix = wmat;
        }

        public void FloorPlane()
        {
            var dataStream = _vertexRenderer.LockVertexBuffer();

            _floorSize = 2;

            dataStream.Write(new TexturedColouredVertex4(new Vector3(-_floorSize, -_floorSize, 0), new Vector2(0, 1), Colours.White));
            dataStream.Write(new TexturedColouredVertex4(new Vector3(-_floorSize, _floorSize, 0), new Vector2(0, 0), Colours.White));
            dataStream.Write(new TexturedColouredVertex4(new Vector3(_floorSize, _floorSize, 0), new Vector2(1, 0), Colours.White));

            dataStream.Write(new TexturedColouredVertex4(new Vector3(-_floorSize, -_floorSize, 0), new Vector2(0, 1), Colours.White));
            dataStream.Write(new TexturedColouredVertex4(new Vector3(_floorSize, _floorSize, 0), new Vector2(1, 0), Colours.White));
            dataStream.Write(new TexturedColouredVertex4(new Vector3(_floorSize, -_floorSize, 0), new Vector2(1, 1), Colours.White));

            _vertexRenderer.UnlockVertexBuffer();

            SetTexture(_graphicsContext.GetTexture("mesh.png"));

            _graphicsContext.AlphaTest = true;
            _graphicsContext.AlphaBlending = true;
            _graphicsContext.AlphaRef = 1;
            _graphicsContext.AlphaFunc = CompareFunc.GreaterEqual;

            _effect.CommitChanges();

            _vertexRenderer.RenderForShader(PrimitiveType.TriangleList, 0, 2);
        }
    }
}