using Cysharp.Threading.Tasks;
using Template.Features.DataStorage;

namespace Template.Features.Vibration
{
    public class VibrationService : IVibrationService
    {
        private ISaveLoadService _saveLoadService;
        private VibrationUserModel _userModel;
        
        public VibrationService(ISaveLoadService saveLoadService)
        {
            _saveLoadService = saveLoadService;
        }

        public async UniTask InitAsync()
        {
            _userModel = await _saveLoadService.LoadOrDefault<VibrationUserModel>();
        }

        public bool Enabled()
        {
            return _userModel.Enabled;
        }

        public void SetEnable(bool enable)
        {
            _userModel.Enabled = enable;
            _saveLoadService.Save(_userModel);
        }
    }
}