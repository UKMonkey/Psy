using System;
using Psy.Core.Logging;
using SlimDX.Direct3D9;
using SlimDX.XAudio2;

namespace Psy.Graphics.DirectX
{
    public static class SlimDXUtils
    {
        public static void DumpObjectTable()
        {
            var objectTable = SlimDX.ObjectTable.Objects;

            foreach (var comObject in objectTable)
            {
                if (comObject is Texture)
                {
                    var texture = (Texture) comObject;
                    Logger.Write(string.Format("{2} AA.Texture `{0}`, IsDefaultPool: ---{1}---", texture.DebugName, texture.IsDefaultPool, texture.Disposed));
                }
                else if (comObject is CubeTexture)
                {
                    var texture = (CubeTexture)comObject;
                    Logger.Write(string.Format("{2} AI.CubeTexture `{0}`, IsDefaultPool: ---{1}---", texture.DebugName, texture.IsDefaultPool, texture.Disposed));
                }
                else if (comObject is Effect)
                {
                    var effect = (Effect) comObject;
                    Logger.Write(string.Format("{2} AB.Effect `{0}`, IsDefaultPool: ---{1}---", effect.Tag, effect.IsDefaultPool, effect.Disposed));
                }
                else if (comObject is Font)
                {
                    var font = (Font) comObject;
                    Logger.Write(string.Format("{2} AC.Font `{0}`, IsDefaultPool: ---{1}---", font.Tag, font.IsDefaultPool, font.Disposed));
                }
                else if (comObject is VertexBuffer)
                {
                    var vertexBuffer = (VertexBuffer)comObject;
                    Logger.Write(string.Format("{2} AD.VertexBuffer `{0}`, IsDefaultPool: ---{1}--- ", vertexBuffer.Tag, vertexBuffer.IsDefaultPool, vertexBuffer.Disposed));
                }
                else if (comObject is VertexDeclaration)
                {
                    var vertexDec = (VertexDeclaration)comObject;
                    Logger.Write(string.Format("{2} AE.VertexDeclaration `{0}`, IsDefaultPool: ---{1}---", vertexDec.Tag, vertexDec.IsDefaultPool, vertexDec.Disposed));
                }
                else if (comObject is Surface)
                {
                    var surface = (Surface)comObject;
                    Logger.Write(string.Format("{2} AF.Surface `{0}`, IsDefaultPool: ---{1}---", surface.Tag, surface.IsDefaultPool, surface.Disposed));
                }
                else if (comObject is Direct3D)
                {
                    
                }
                else if (comObject is Device)
                {

                }
                else if (comObject is XAudio2)
                {
                    var audio = (XAudio2)comObject;
                    Logger.Write(string.Format("{3} AH.XAudio2 `{0}`, IsDefaultPool: ---{1}--- , Created:{2}", audio.Tag, audio.IsDefaultPool, audio.CreationSource, audio.Disposed));
                }
                else
                {
                    throw new Exception("No idea what this SlimDX object is.");
                }
            }
        }
    }
}