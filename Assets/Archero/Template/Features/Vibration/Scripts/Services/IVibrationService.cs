namespace Template.Features.Vibration
{
    public interface IVibrationService
    {
        public bool Enabled();
        public void SetEnable(bool enable);
    }
}