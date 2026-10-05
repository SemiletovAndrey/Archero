using Template.Features.DataStorage;

namespace Template.Features.Vibration
{
    public class VibrationService : IVibrationService
    {
        private IDataStorageService _dataStorageService;
        private VibrationUserModel _userModel;
        
        public VibrationService(IDataStorageService dataStorageService)
        {
            _dataStorageService = dataStorageService;
        }

        public void Init()
        {
            _userModel = _dataStorageService.LoadOrDefault<VibrationUserModel>();
        }

        public bool Enabled()
        {
            return _userModel.Enabled;
        }

        public void SetEnable(bool enable)
        {
            _userModel.Enabled = enable;
            _dataStorageService.Save(_userModel);
        }
    }
}