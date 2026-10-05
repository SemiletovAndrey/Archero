using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Template.Features.ScreenNavigator
{
    public class NavigationCommand<TItem, TPayload> : INavigationCommand
        where TItem : BaseNavigationItem<TPayload>
        where TPayload : BaseNavigationItemPayload
    {
        private TPayload _payload;
        private UniTaskCompletionSource _commandAwaiter;

        public NavigationCommand(TPayload payload)
        {
            _payload = payload;
            _commandAwaiter = new UniTaskCompletionSource();
        }

        public UniTask GetCommandAwaiter()
        {
            return _commandAwaiter.Task;
        }

        public async UniTask Execute(ScreenNavigator screenNavigator)
        {
            try
            {
                await screenNavigator.NavigateToFromCommand<TItem, TPayload>(_payload);
                _commandAwaiter.TrySetResult();
            }
            catch (Exception exp)
            {
                Debug.LogError($"Error execute navigation command: {exp.Message}");
            }
        }
    }
}