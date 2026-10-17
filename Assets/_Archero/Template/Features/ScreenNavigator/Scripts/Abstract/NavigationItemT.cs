using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Scripts.Utils;
using Zenject;

namespace Template.Features.ScreenNavigator
{
    public abstract class BaseNavigationItem<TPayload> : BaseNavigationItem where TPayload : BaseNavigationItemPayload
    {
        private IClassFactory _classFactory;

        /// <summary>Элемент получил данные (целевой элемент при переходе или повторный выбор уже открытого).</summary>
        public event Func<TPayload, UniTask> Activated;

        /// <summary>Элемент открыт (срабатывает для каждого элемента пути, не только для целевого).</summary>
        public event Func<NavigationItemOpenPayload, UniTask> Opened;

        /// <summary>Элемент закрывается.</summary>
        public event Func<NavigationItemClosePayload, UniTask> Closed;

        /// <summary>Прежнее имя события Activated.</summary>
        [Obsolete("Use Activated")]
        public event Func<TPayload, UniTask> Initialized
        {
            add => Activated += value;
            remove => Activated -= value;
        }

        [Inject]
        private void Construct(IClassFactory classFactory)
        {
            _classFactory = classFactory;
        }

        public override void Init()
        {
            NavigationItemList itemList = AddInnerItems();
            CreateInnerItems(itemList);
        }

        protected virtual NavigationItemList AddInnerItems()
        {
            return new NavigationItemList();
        }

        public override async UniTask MoveTo<TItem, TItemData>(TItemData data, NavigationItemOpenPayload payload)
        {
            // Помечаем элемент открытым сразу: если Activate или обработчик упадет, навигатор вызовет Deactivate.
            IsActive = true;

            if (GetType() == typeof(TItem))
            {
                TPayload openPayload = (TPayload)(object)data;
                await Activate(openPayload);
            }

            await Opened.InvokeAllAsync(payload);

            await base.MoveTo<TItem, TItemData>(data, payload);
        }

        public override async UniTask Deactivate(NavigationItemClosePayload payload)
        {
            try
            {
                await Closed.InvokeAllAsync(payload);
                await base.Deactivate(payload);
            }
            finally
            {
                IsActive = false;
            }
        }

        public virtual async UniTask Activate(TPayload data)
        {
            await Activated.InvokeAllAsync(data);
        }

        internal override UniTask Refresh(BaseNavigationItemPayload data)
        {
            if (data is not TPayload typed)
                throw new InvalidCastException(
                    $"{GetType().Name} expects payload {typeof(TPayload).Name}, " +
                    $"got {(data == null ? "null" : data.GetType().Name)}.");

            return Activate(typed);
        }

        private void CreateInnerItems(NavigationItemList itemList)
        {
            InnerItems = itemList
                .Select(type =>
                {
                    BaseNavigationItem innerItem = (BaseNavigationItem)_classFactory.CreateAndBind(type);
                    innerItem.Init();

                    return innerItem;
                })
                .ToList();
        }
    }
}