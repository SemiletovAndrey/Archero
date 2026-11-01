namespace DataEditorFramework.Core.Services
{
    public interface ISerializationService
    {
        string Serialize(object obj);
        T Deserialize<T>(string json);
    }
}