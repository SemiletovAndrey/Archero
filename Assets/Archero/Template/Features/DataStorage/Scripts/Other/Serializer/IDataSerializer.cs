namespace Template.Features.DataStorage
{
    public interface IDataSerializer
    {
        public string Serialize<TData>(TData data);
        public TData Deserialize<TData>(string serializedData);
    }
}