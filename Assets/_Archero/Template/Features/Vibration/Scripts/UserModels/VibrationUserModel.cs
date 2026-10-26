using Template.Features.DataStorage;

namespace Template.Features.Vibration
{
    public class VibrationUserModel : ISaveData
    {
        public bool Enabled { get; set; } = true;
    }
}