using System.Collections.Generic;

namespace Psy.Core.ThreedMesh
{
    public class ModelObject
    {
        public int MaterialIndex;
        public readonly List<Face> Faces;

        public ModelObject()
        {
            Faces = new List<Face>();
        }
    }
}