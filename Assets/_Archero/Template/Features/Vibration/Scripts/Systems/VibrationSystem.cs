using UnityEngine;

namespace Template.Features.Vibration
{
    public class VibrationSystem : IVibrationSystem
    {
        private readonly IVibrationService _vibrationService;
        private IVibrationHandler _vibrationHandler;

        public VibrationSystem(IVibrationService vibrationService)
        {
            _vibrationService = vibrationService;
        }

        public void Init(IVibrationHandler vibrationHandler)
        {
            _vibrationHandler = vibrationHandler;
        }

        public void PlayLightVibration()
        {
            if (!_vibrationService.Enabled())
            {
                return;
            }
            
            _vibrationHandler.PlayLightVibration();
        }

        public void PlayMediumVibration()
        {
            if (!_vibrationService.Enabled())
            {
                return;
            }
            
            _vibrationHandler.PlayMediumVibration();
        }

        public void PlayHeavyVibration()
        {
            if (!_vibrationService.Enabled())
            {
                return;
            }
            
            _vibrationHandler.PlayHeavyVibration();
        }
    }
}