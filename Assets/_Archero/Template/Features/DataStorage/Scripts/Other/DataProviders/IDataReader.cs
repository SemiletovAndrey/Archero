namespace Template.Features.DataStorage
{
    public interface IDataReader<TData> where TData : ISaveData
    {
        public void ReadFrom(TData data);
    }
}