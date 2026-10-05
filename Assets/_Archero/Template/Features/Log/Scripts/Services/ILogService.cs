namespace Template.Features.Log
{
    public interface ILogService
    {
        public void AddLogHandler(ILogHandler logHandler);
        public void RemoveLogHandler(ILogHandler logHandler);
    }
}