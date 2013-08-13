namespace Psy.Core
{
    public class Material
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string TextureName;
        public readonly string WalkSoundName;
        public readonly bool Outside;

        public Material(int id, string name, string textureName, string walkSoundName, bool outside)
        {
            Id = id;
            Name = name;
            TextureName = textureName;
            WalkSoundName = walkSoundName;
            Outside = outside;
        }
    }
}