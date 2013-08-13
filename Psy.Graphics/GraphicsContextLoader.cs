using System;
using System.Linq;
using System.Reflection;
using Psy.Core;

namespace Psy.Graphics
{
    public enum Renderer
    {
        OpenGL,
        DirectX
    }

    public static class GraphicsContextLoader
    {
        public static GraphicsContext Create(Renderer renderer, WindowAttributes windowAttributes)
        {
            var path = string.Format("Psy.Graphics.{0}.dll", renderer);


            var assembly = Assembly.LoadFrom(path);

            var type = assembly.GetTypes().Single(x => typeof (IGraphicsContextFactory).IsAssignableFrom(x));
            var constructorInfo = type.GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                throw new Exception("Implementation of IGraphicsContextFactory must have parameterless constructor");
            }

            var impl = constructorInfo.Invoke(null);

            var method = type.GetMethod("Create");
            var result = (GraphicsContext)method.Invoke(impl, new object[] {windowAttributes});

            return result;
        }
    }
}