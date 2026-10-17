using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using Template.Features.UIViewFactory;

namespace Template.Features.ScreenNavigator
{
    public abstract class ScreenFactoryBase : UIViewFactoryBase<BaseScreen, ScreenEntryBase>, IScreenFactory
    {
        protected ScreenFactoryBase(IClassFactory classFactory, IUILayersService layersService)
            :
            base(classFactory, layersService)
        {
        }

        protected override UILayerType Layer => UILayerType.Screen;

        protected override int GetKey(ScreenEntryBase entry)
        {
            return (int)entry.Type;
        }

        protected override bool IsPooled(ScreenEntryBase entry)
        {
            return entry.Pooled;
        }

        protected override bool HasSource(ScreenEntryBase entry)
        {
            return entry.HasSource;
        }

        protected override string Describe(ScreenEntryBase entry)
        {
            return $"ScreenType.{entry.Type}";
        }

        protected override void OnViewCreated(BaseScreen view, ScreenEntryBase entry)
        {
            view.Init(entry.Type);

            // Экран невидим, пока ему не передали данные: видимым его делает активатор перед Open.
            view.SetVisible(false);
        }

        protected override void OnViewReused(BaseScreen view, ScreenEntryBase entry)
        {
            view.SetVisible(false);
        }
    }
}