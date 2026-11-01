namespace Template.Features.DataStorage
{
    public interface IDataWriter<TData> where TData : ISaveData
    {
        public void WriteTo(TData data);
    }
}