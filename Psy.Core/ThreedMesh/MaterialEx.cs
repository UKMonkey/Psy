using Psy.Core.ThreedMesh.Reader;

namespace Psy.Core.ThreedMesh
{
    public class MaterialEx
    {
        public AmbientLightSource AmbientLightSource;
        public string FriendlyName;
        public string MaterialName;

        public MaterialEx()
        {
            AmbientLightSource = AmbientLightSource.Sunlight;
        }
    }
}