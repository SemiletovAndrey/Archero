namespace Template.Features.ScreenNavigator
{
    public interface IScenePayloadProvider<out TPayload> where TPayload : BaseNavigationItemPayload
    {
        public TPayload GetPayload();
    }
}