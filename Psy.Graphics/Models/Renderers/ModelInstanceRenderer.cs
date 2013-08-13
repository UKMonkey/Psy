using System;
using System.Collections.Generic;
using Psy.Graphics.Effects;
using Psy.Graphics.VertexDeclarations;

namespace Psy.Graphics.Models.Renderers
{
    public class ModelInstanceRenderer : IDisposable
    {
        private readonly GraphicsContext _graphicsContext;

        private readonly Dictionary<int, IVertexRenderer<TexturedColouredVertex4>> _vertexRenderers;

        private string _lastTextureName;

        public ModelInstanceRenderer(
            GraphicsContext graphicsContext)
        {
            _graphicsContext = graphicsContext;
            _vertexRenderers = new Dictionary<int, IVertexRenderer<TexturedColouredVertex4>>();
            _lastTextureName = null;
        }

        private IVertexRenderer<TexturedColouredVertex4> GetRendererFor(MeshInstance meshInstance)
        {
            IVertexRenderer<TexturedColouredVertex4> renderer;

            if (_vertexRenderers.TryGetValue(meshInstance.TriangleCount, out renderer))
            {
                return renderer;
            }

            renderer = _graphicsContext.CreateVertexRenderer<TexturedColouredVertex4>(meshInstance.TriangleCount * 3);

            _vertexRenderers[meshInstance.TriangleCount] = renderer;

            return renderer;
        }

        public void Dispose()
        {
            foreach (var vertexRenderer in _vertexRenderers)
            {
                vertexRenderer.Value.Dispose();
            }
        }

        public void BeginRender()
        {
            _lastTextureName = null;
        }

        public void Render(ModelInstance modelInstance, IEffect effect)
        {
            foreach (var meshInstance in modelInstance.MeshInstances)
            {
                var renderer = GetRendererFor(meshInstance);

                var dataStream = renderer.LockVertexBuffer();
                dataStream.WriteRange(meshInstance.VertexBuffer);
                renderer.UnlockVertexBuffer();

                if (meshInstance.TextureArea == null)
                {
                    meshInstance.TextureArea = _graphicsContext.GetTexture(meshInstance.TextureName);
                }

                if (meshInstance.TextureName != _lastTextureName)
                {
                    _lastTextureName = meshInstance.TextureName;

                    effect.SetTexture("tex0", meshInstance.TextureArea);
                    effect.CommitChanges();
                }

                renderer.RenderForShader(PrimitiveType.TriangleList, 0, meshInstance.TriangleCount);
            }
        }

        public void RenderNoTexture(ModelInstance modelInstance)
        {
            foreach (var meshInstance in modelInstance.MeshInstances)
            {
                var vertexRenderer = GetRendererFor(meshInstance);

                var dataStream = vertexRenderer.LockVertexBuffer();
                dataStream.WriteRange(meshInstance.VertexBuffer);
                vertexRenderer.UnlockVertexBuffer();

                vertexRenderer.RenderForShader(PrimitiveType.TriangleList, 0, meshInstance.TriangleCount);
            }
        }

        public void EndRender()
        {
            _lastTextureName = null;
        }
    }
}