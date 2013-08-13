using System.IO;

namespace Psy.Core.ThreedMesh.Reader
{
    public interface IModelReader
    {
        Model ReadFromStream(Stream stream);
    }
}