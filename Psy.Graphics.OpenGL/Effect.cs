using System;
using OpenTK.Graphics.OpenGL;
using Psy.Core;
using Psy.Core.Logging;
using Psy.Graphics.Effects;
using SlimMath;

namespace Psy.Graphics.OpenGL
{
    public class Effect : IEffect
    {
        private readonly OpenGLGraphicsContext _openGLGraphicsContext;
        private readonly int _programHandle;
        private int _vertexShaderHandle;
        private int _fragmentShaderHandle;
        private string _vertexShaderText;
        private string _fragmentShaderText;

        private Effect(OpenGLGraphicsContext openGLGraphicsContext,
            int programHandle, int vertexShaderHandle, int fragmentShaderHandle, 
            string vertexShaderText, string fragmentShaderText)
        {
            _openGLGraphicsContext = openGLGraphicsContext;
            _programHandle = programHandle;
            _vertexShaderHandle = vertexShaderHandle;
            _fragmentShaderHandle = fragmentShaderHandle;
            _vertexShaderText = vertexShaderText;
            _fragmentShaderText = fragmentShaderText;
        }

        public IEffectHandle Technique { set; private get; }

        public IEffectHandle CreateHandle(string name)
        {
            return new EffectHandle();
        }

        public void SetValue<T>(string name, T value) where T : struct
        {

        }

        public void SetMatrix(string name, Matrix value)
        {
            var inputLocation = GL.GetUniformLocation(_programHandle, name);
            SetMatrix(inputLocation, value);
        }

        public void SetMatrix(IEffectHandle effectHandle, Matrix value)
        {
            SetMatrix(((EffectHandle)effectHandle).Index, value);
        }

        private void SetMatrix(int inputLocation, Matrix value)
        {
            GL.UniformMatrix4(inputLocation, 1, false, ref value.M11);
        }

        public void SetValue<T>(IEffectHandle effectHandle, T value) where T : struct
        {

        }

        public void SetTexture(string name, TextureAreaHolder texture)
        {
            var inputLocation = GL.GetUniformLocation(_programHandle, name);
            SetTexture(inputLocation, texture);
        }

        public void SetTexture(IEffectHandle effectHandle, TextureAreaHolder texture)
        {
            SetTexture(((EffectHandle)effectHandle).Index, texture);
        }

        private void SetTexture(int inputLocation, TextureAreaHolder texture)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture.TextureArea.TextureId);
            GL.Uniform1(inputLocation, 0); GLH.AssertGLError();
        }

        public void SetTexture(IEffectHandle effectHandle, ICubeTexture texture)
        {

        }

        public void SetTexture(string name, ICubeTexture cubeTexture)
        {
            
        }

        public void Begin()
        {
            GL.UseProgram(_programHandle);
        }

        public void BeginPass(int i)
        {
            
        }

        public void EndPass()
        {
            
        }

        public void End()
        {
            GL.UseProgram(0);
        }

        public void CommitChanges()
        {
            
        }

        public void Reload()
        {
            
        }

        internal static Effect Create(OpenGLGraphicsContext context, string vertexShaderText, string fragmentShaderText)
        {
            Logger.Write("Compiling shader", LoggerLevel.Debug);

            var vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            var fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShaderHandle, vertexShaderText);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderText);

            GL.CompileShader(vertexShaderHandle);
            GL.CompileShader(fragmentShaderHandle);

            var vertexShaderCompileLog = GL.GetShaderInfoLog(vertexShaderHandle);
            Logger.Write(vertexShaderCompileLog, LoggerLevel.Debug);

            int vertexShaderCompileStatus;
            GL.GetShader(vertexShaderHandle, ShaderParameter.CompileStatus, out vertexShaderCompileStatus);
            if (vertexShaderCompileStatus != 1)
            {
                throw new Exception("Failed to compile vertex shader");
            }

            int fragmentShaderCompileStatus;
            GL.GetShader(fragmentShaderHandle, ShaderParameter.CompileStatus, out fragmentShaderCompileStatus);
            if (fragmentShaderCompileStatus != 1)
            {
                throw new Exception("Failed to compile fragment shader");
            }

            var fragmentShaderCompileLog = GL.GetShaderInfoLog(fragmentShaderHandle);
            Logger.Write(fragmentShaderCompileLog, LoggerLevel.Debug);

            var programHandle = GL.CreateProgram();
            GL.AttachShader(programHandle, vertexShaderHandle);
            GL.AttachShader(programHandle, fragmentShaderHandle);

            GL.LinkProgram(programHandle);

            var linkOutput = GL.GetProgramInfoLog(programHandle);
            Logger.Write(linkOutput, LoggerLevel.Debug);

            int linkStatus;
            GL.GetProgram(programHandle, ProgramParameter.LinkStatus, out linkStatus);

            if (linkStatus != 1)
            {
                throw new Exception("Failed to link shader program");
            }

            var effect = new Effect(context, programHandle, vertexShaderHandle, 
                fragmentShaderHandle, vertexShaderText, fragmentShaderText);

            return effect;

        }
    }
}