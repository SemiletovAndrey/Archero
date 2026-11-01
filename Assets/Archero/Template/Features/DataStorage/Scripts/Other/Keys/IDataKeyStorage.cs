namespace Template.Features.DataStorage
{
    public interface IDataKeyStorage
    {
        public string GetKeyFor<TData>() where TData : ISaveData;
    }
}