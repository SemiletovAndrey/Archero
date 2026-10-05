namespace Template.Features.ScreenNavigator
{
    public abstract class SceneNavigationItem<TPayload> : SceneSelectorNavigationItem<TPayload> 
        where TPayload : BaseNavigationItemPayload
    {
        public abstract string SceneName { get; }

        protected override string GetDefaultSceneName()
        {
            return SceneName;
        }
    }
}