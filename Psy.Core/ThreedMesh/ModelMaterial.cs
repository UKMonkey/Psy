using SlimMath;

namespace Psy.Core.ThreedMesh
{
    public class ModelMaterial
    {
        public int MaterialId;
        public string MaterialName;
        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public string DiffuseTexture { get; set; }
        public MaterialEx MaterialEx;

        public ModelMaterial()
        {
            MaterialEx = new MaterialEx();
        }
    }
}