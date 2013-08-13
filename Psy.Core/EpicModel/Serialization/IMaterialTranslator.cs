namespace Psy.Core.EpicModel.Serialization
{
    public interface IMaterialTranslator 
    {
        string Translate(int materialId);
        int Translate(string textureName);
    }
}