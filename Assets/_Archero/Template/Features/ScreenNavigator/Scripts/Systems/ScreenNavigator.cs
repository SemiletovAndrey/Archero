using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace Template.Features.ScreenNavigator
{
    public class ScreenNavigator : IScreenNavigator
    {
        private readonly IMainNavigationItem _mainNavigationItem;

        private readonly Queue<INavigationCommand> _navigationCommands = new();
        private List<BaseNavigationItem> _lastPath;
        
        public ScreenNavigator(IMainNavigationItem mainNavigationItem)
        {
            _mainNavigationItem = mainNavigationItem;
        }

        public void Init()
        {
            _mainNavigationItem.Init();
        }
        
        public UniTask NavigateTo<TItem, TPayload>(TPayload payload)
            where TItem : BaseNavigationItem<TPayload>
            where TPayload : BaseNavigationItemPayload
        {
            NavigationCommand<TItem, TPayload> command = new(payload);
            _navigationCommands.Enqueue(command);

            if (_navigationCommands.Count == 1)
            {
                ProcessCommands().Forget();
            }

            return command.GetCommandAwaiter();
        }

        public UniTask NavigateTo<TItem>() where TItem : BaseNavigationItem<EmptyNavigationItemPayload>
        {
            EmptyNavigationItemPayload payload = new();
            return NavigateTo<TItem, EmptyNavigationItemPayload>(payload);
        }
        
        public UniTask NavigateToFromCommand<TItem, TPayload>(TPayload payload)
            where TItem : BaseNavigationItem<TPayload>
            where TPayload : BaseNavigationItemPayload
        {
            return Navigate<TItem, TPayload>(payload);
        }

        private async UniTask ProcessCommands()
        {
            while (_navigationCommands.Count > 0)
            {
                INavigationCommand command = _navigationCommands.Peek();
                await command.Execute(this);
                
                _navigationCommands.Dequeue();
            }
        }
        
        private async UniTask Navigate<TItem, TPayload>(TPayload payload)
            where TItem : BaseNavigationItem<TPayload>
            where TPayload : BaseNavigationItemPayload
        {
            if (_lastPath is null)
            {
                NavigationItemOpenPayload openPayload = new(new List<BaseNavigationItem>());
                await _mainNavigationItem.MoveTo<TItem, TPayload>(payload, openPayload);

                _lastPath = _mainNavigationItem.GetItemPath<TItem>();
            }
            else
            {
                
                _lastPath = await OpenWithPath<TItem, TPayload>(payload);
            }
        }
        
        private async UniTask<List<BaseNavigationItem>> OpenWithPath<TItem, TData>(TData data)
            where TItem : BaseNavigationItem<TData>
            where TData : BaseNavigationItemPayload
        {
            List<BaseNavigationItem> path = _mainNavigationItem.GetItemPath<TItem>();

            int skipAmount = 0;
            int min = Math.Min(_lastPath.Count, path.Count);
            for (int i = 0; i < min; i++)
            {
                if (_lastPath[i] == path[i])
                {
                    skipAmount++;
                }
            }

            List<BaseNavigationItem> openingItems = path.Skip(skipAmount).ToList();
            List<BaseNavigationItem> closingItems = _lastPath.Skip(skipAmount).Reverse().ToList();

            await ChangePanels<TItem, TData>(closingItems, openingItems, data);

            return path;
        }

        private async UniTask ChangePanels<TItem, TData>(List<BaseNavigationItem> closingItems,
            List<BaseNavigationItem> openingItems, TData data)
            where TItem : BaseNavigationItem<TData>
            where TData : BaseNavigationItemPayload
        {
            foreach (BaseNavigationItem item in closingItems)
            {
                NavigationItemClosePayload payload = new(openingItems);
                await item.Deactivate(payload);
            }
            
            if (openingItems.Count > 0)
            {
                NavigationItemOpenPayload payload = new(closingItems);
                BaseNavigationItem openingItemParent = openingItems[0];
                
                await openingItemParent.MoveTo<TItem, TData>(data, payload);
            }
        }
    }
}