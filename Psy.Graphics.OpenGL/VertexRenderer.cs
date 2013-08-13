using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using Psy.Core;
using SlimMath;

namespace Psy.Graphics.OpenGL
{
    public class VertexRenderer<T> : IVertexRenderer<T> where T : struct
    {
        private readonly OpenGLGraphicsContext _openGLGraphicsContext;
        internal int BufferId;
        private DataStream<T> _dataStream;
        internal readonly T[] LocalCopy;
        private TextureArea _activeTextureArea;
        private bool _previousTextureEnableState;

        internal VertexRenderer(OpenGLGraphicsContext openGLGraphicsContext, int vertexCount)
        {
            _openGLGraphicsContext = openGLGraphicsContext;
            LocalCopy = new T[vertexCount];

            GL.GenBuffers(1, out BufferId); GLH.AssertGLError();
        }

        public void Dispose()
        {
            GL.DeleteBuffers(1, ref BufferId); GLH.AssertGLError();
        }

        public IDataStream<T> LockVertexBuffer()
        {
            if (_dataStream != null)
            {
                throw new Exception("Vertex buffer is already locked");
            }

            _dataStream = new DataStream<T>(this);
            return _dataStream;
        }

        public void UnlockVertexBuffer()
        {
            if (_dataStream == null)
            {
                throw new Exception("Vertex buffer is not locked");
            }

            _dataStream.Flush();
            _dataStream = null;
        }

        public IDataStream<short> LockIndexBuffer()
        {
            throw new NotImplementedException();
        }

        public void UnlockIndexBuffer()
        {
            throw new NotImplementedException();
        }

        public void Render(PrimitiveType primitiveType, int startIndex, int primitiveCount)
        {
            Debug.Assert(_dataStream == null);
            _activeTextureArea = null;
            RenderImpl(primitiveType, startIndex, primitiveCount);
        }

        public void RenderIndexed(PrimitiveType primitiveType, int primitiveCount)
        {
            Debug.Assert(_dataStream == null);
            _activeTextureArea = null;
            throw new NotImplementedException();
        }

        public void RenderNonIndexed(PrimitiveType primitiveType, int startIndex, int primitiveCount)
        {
            Debug.Assert(_dataStream == null);
            _activeTextureArea = null;
            RenderImpl(primitiveType, startIndex, primitiveCount);
        }

        public void RenderNoTexture(PrimitiveType primitiveType, int startIndex, int primitiveCount)
        {
            Debug.Assert(_dataStream == null);
            _activeTextureArea = null;
            RenderImpl(primitiveType, startIndex, primitiveCount);
        }

        public void RenderForShader(PrimitiveType primitiveType, int startIndex, int primitiveCount)
        {
            Debug.Assert(_dataStream == null);

            _previousTextureEnableState = GL.IsEnabled(EnableCap.Texture2D);
            GL.Enable(EnableCap.Texture2D); GLH.AssertGLError();

            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferId); GLH.AssertGLError(); GLH.AssertGLError();

            VertexDataPointerRobot.SetGLState<T>();

            var projMat = Matrix.Identity;
            var modelView = Matrix.Identity;
            var zCompareFunc = ZCompareFunction.Always;

            if (VertexDataPointerRobot.PreTransformed)
            {
                GL.GetFloat(GetPName.ProjectionMatrix, out projMat.M11); GLH.AssertGLError();
                GL.GetFloat(GetPName.ModelviewMatrix, out modelView.M11); GLH.AssertGLError();

                zCompareFunc = _openGLGraphicsContext.ZCompareFunctionFunction;
                _openGLGraphicsContext.ZCompareFunctionFunction = ZCompareFunction.Less;

                var newProjMatrix = Matrix.OrthoOffCenterLH(
                    0,
                    _openGLGraphicsContext.WindowSize.Width,
                    _openGLGraphicsContext.WindowSize.Height,
                    0,
                    -1,
                    1);
                _openGLGraphicsContext.Projection = newProjMatrix;
                _openGLGraphicsContext.World = Matrix.Identity;
                _openGLGraphicsContext.View = Matrix.Identity;
            }

            GL.DrawArrays(
                MapPrimitiveType(primitiveType),
                startIndex,
                GetIndicesCount(primitiveType, primitiveCount));
            GLH.AssertGLError();

            if (VertexDataPointerRobot.PreTransformed)
            {
                _openGLGraphicsContext.Projection = projMat;
                GL.MatrixMode(MatrixMode.Modelview); GLH.AssertGLError();
                GL.LoadMatrix(ref modelView.M11); GLH.AssertGLError();

                _openGLGraphicsContext.ZCompareFunctionFunction = zCompareFunc;
            }

            ResetTextureState();

            GL.DisableClientState(ArrayCap.ColorArray); GLH.AssertGLError();
            GL.DisableClientState(ArrayCap.TextureCoordArray); GLH.AssertGLError();
            GL.DisableClientState(ArrayCap.VertexArray); GLH.AssertGLError();
        }

        /// <summary>
        /// Render with texture
        /// </summary>
        /// <param name="primitiveType"></param>
        /// <param name="startIndex"></param>
        /// <param name="primitiveCount"></param>
        /// <param name="textureArea"></param>
        /// <param name="alpha"></param>
        /// <param name="intensity"></param>
        public void Render(PrimitiveType primitiveType, int startIndex, int primitiveCount,
            TextureArea textureArea, float alpha, float intensity)
        {
            // todo: handle alpha, and intensity
            Debug.Assert(_dataStream == null);
            _activeTextureArea = textureArea;
            RenderImpl(primitiveType, startIndex, primitiveCount);
        }

        private void RenderImpl(PrimitiveType primitiveType, int startIndex, int primitiveCount)
        {
            SetTextureState();

            GL.BindBuffer(BufferTarget.ArrayBuffer, BufferId); GLH.AssertGLError();

            VertexDataPointerRobot.SetGLState<T>();

            var projMat = Matrix.Identity;
            var modelView = Matrix.Identity;
            var zCompareFunc = ZCompareFunction.Always;

            if (VertexDataPointerRobot.PreTransformed)
            {
                GL.GetFloat(GetPName.ProjectionMatrix, out projMat.M11); GLH.AssertGLError();
                GL.GetFloat(GetPName.ModelviewMatrix, out modelView.M11); GLH.AssertGLError();

                zCompareFunc = _openGLGraphicsContext.ZCompareFunctionFunction;
                _openGLGraphicsContext.ZCompareFunctionFunction = ZCompareFunction.Less;

                var newProjMatrix = Matrix.OrthoOffCenterLH(
                    0,
                    _openGLGraphicsContext.WindowSize.Width,
                    _openGLGraphicsContext.WindowSize.Height,
                    0,
                    -1,
                    1); GLH.AssertGLError();
                _openGLGraphicsContext.Projection = newProjMatrix;
                _openGLGraphicsContext.World = Matrix.Identity;
                _openGLGraphicsContext.View = Matrix.Identity;
            }

            GL.DrawArrays(
                MapPrimitiveType(primitiveType),
                startIndex,
                GetIndicesCount(primitiveType, primitiveCount));
            GLH.AssertGLError();

            if (VertexDataPointerRobot.PreTransformed)
            {
                _openGLGraphicsContext.Projection = projMat;
                GL.MatrixMode(MatrixMode.Modelview); GLH.AssertGLError();
                GL.LoadMatrix(ref modelView.M11); GLH.AssertGLError();

                _openGLGraphicsContext.ZCompareFunctionFunction = zCompareFunc;
            }

            ResetTextureState();

            GL.DisableClientState(ArrayCap.ColorArray); GLH.AssertGLError();
            GL.DisableClientState(ArrayCap.TextureCoordArray); GLH.AssertGLError();
            GL.DisableClientState(ArrayCap.VertexArray); GLH.AssertGLError();
        }

        private void ResetTextureState()
        {
            if (_previousTextureEnableState)
            {
                GL.Enable(EnableCap.Texture2D); GLH.AssertGLError();
            }
            else
            {
                GL.Disable(EnableCap.Texture2D); GLH.AssertGLError();
            }
        }

        private void SetTextureState()
        {
            _previousTextureEnableState = GL.IsEnabled(EnableCap.Texture2D);

            if (_activeTextureArea == null)
            {
                GL.Disable(EnableCap.Texture2D); GLH.AssertGLError();
            }
            else
            {
                GL.Enable(EnableCap.Texture2D); GLH.AssertGLError();
                GL.BindTexture(TextureTarget.Texture2D, _activeTextureArea.TextureId); GLH.AssertGLError();
            }
        }

        private static BeginMode MapPrimitiveType(PrimitiveType primitiveType)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                    return BeginMode.Points;
                case PrimitiveType.LineList:
                    return BeginMode.Lines;
                case PrimitiveType.LineStrip:
                    return BeginMode.LineStrip;
                case PrimitiveType.TriangleList:
                    return BeginMode.Triangles;
                case PrimitiveType.TriangleStrip:
                    return BeginMode.TriangleStrip;
                case PrimitiveType.TriangleFan:
                    return BeginMode.TriangleFan;
                default:
                    throw new ArgumentOutOfRangeException("primitiveType");
            }
        }

        private int GetIndicesCount(PrimitiveType primitiveType, int primitiveCount)
        {
            switch (primitiveType)
            {
                case PrimitiveType.PointList:
                    return primitiveCount;
                case PrimitiveType.LineList:
                    return primitiveCount * 2;
                case PrimitiveType.LineStrip:
                    return 2 + (primitiveCount - 1);
                case PrimitiveType.TriangleList:
                    return primitiveCount * 3;
                case PrimitiveType.TriangleStrip:
                case PrimitiveType.TriangleFan:
                    return primitiveCount + 2;
                default:
                    throw new ArgumentOutOfRangeException("primitiveType");
            }
        }
    }
}