using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Template.Features.Config.Local;
using UnityEngine;

namespace Template.Features.Popups
{
    public class PopupSystem : IPopupSystem
    {
        private enum PopupState
        {
            Opening,
            Opened,
            Closing
        }
        
        private sealed class PopupSlot
        {
            public readonly PopupType Type;
            public PopupState State = PopupState.Opening;
            public BasePopup Popup;

            public Exception OpenError;

            public readonly UniTaskCompletionSource<BasePopup> Opened = new();

            public readonly UniTaskCompletionSource Closed = new();

            private UniTaskCompletionSource _lastOperation;

            public PopupSlot(PopupType type)
            {
                Type = type;
            }

            public async UniTask RunExclusive(Func<UniTask> operation)
            {
                UniTaskCompletionSource previous = _lastOperation;
                UniTaskCompletionSource current = new UniTaskCompletionSource();
                _lastOperation = current;

                try
                {
                    if (previous != null)
                        await previous.Task;

                    await operation();
                }
                finally
                {
                    current.TrySetResult();
                }
            }

            public UniTask WaitIdle()
            {
                return _lastOperation != null ? _lastOperation.Task : UniTask.CompletedTask;
            }
        }

        private readonly IPopupFactory _factory;
        private readonly ILocalConfigService _localConfigService;

        private readonly Dictionary<PopupType, PopupEntryBase> _entries = new();
        private readonly Dictionary<PopupType, PopupSlot> _slots = new();

        private bool _initialized;
        
        private const int MaxCloseAllPasses = 5;

        public event Action<BasePopup> PopupOpened;
        public event Action<BasePopup> PopupClosed;
        public event Action AllClosed;

        public int OpenedCount => _slots.Count;

        public PopupSystem(ILocalConfigService localConfigService, IPopupFactory factory)
        {
            _localConfigService = localConfigService;
            _factory = factory;
        }

        public bool IsOpen(PopupType popupType)
        {
            return _slots.TryGetValue(popupType, out PopupSlot slot) && slot.State == PopupState.Opened;
        }

        public void Init()
        {
            _entries.Clear();

            PopupConfig config = _localConfigService.GetConfig<PopupConfig>();
            foreach (PopupEntryBase entry in config.Entries)
            {
                RegisterEntry(entry);
            }

            _initialized = true;
        }

        #region Open

        public UniTask Open(PopupType popupType)
        {
            return OpenInternal<BasePopup, EmptyPopupPayload>(popupType, default);
        }

        public UniTask<TPopup> Open<TPopup>() where TPopup : BasePopupT<EmptyPopupPayload>
        {
            return OpenInternal<TPopup, EmptyPopupPayload>(ResolvePopupType<TPopup>(), default);
        }

        public UniTask<TPopup> Open<TPopup, TPayload>(TPayload payload)
            where TPopup : BasePopupT<TPayload>
            where TPayload : IPopupPayload
        {
            return OpenInternal<TPopup, TPayload>(ResolvePopupType<TPopup>(), payload);
        }

        public UniTask<TResult> OpenForResult<TPopup, TResult>()
            where TPopup : BasePopupTR<EmptyPopupPayload, TResult>
        {
            return OpenForResultInternal<TPopup, EmptyPopupPayload, TResult>(default);
        }

        public UniTask<TResult> OpenForResult<TPopup, TPayload, TResult>(TPayload payload)
            where TPopup : BasePopupTR<TPayload, TResult>
            where TPayload : IPopupPayload
        {
            return OpenForResultInternal<TPopup, TPayload, TResult>(payload);
        }

        private async UniTask<TResult> OpenForResultInternal<TPopup, TPayload, TResult>(TPayload payload)
            where TPopup : BasePopupTR<TPayload, TResult>
            where TPayload : IPopupPayload
        {
            TPopup popup = await OpenInternal<TPopup, TPayload>(ResolvePopupType<TPopup>(), payload);

            return await popup.ResultTask;
        }

        private async UniTask<TPopup> OpenInternal<TPopup, TPayload>(PopupType popupType, TPayload payload)
            where TPopup : BasePopup
            where TPayload : IPopupPayload
        {
            EnsureInitialized();

            if (!_entries.TryGetValue(popupType, out PopupEntryBase entry))
                throw new InvalidOperationException($"No prefab configured for PopupType.{popupType}");

            while (true)
            {
                if (!_slots.TryGetValue(popupType, out PopupSlot slot))
                    return await CreateAndOpen<TPopup, TPayload>(popupType, entry, payload);

                switch (slot.State)
                {
                    case PopupState.Opening:
                    {
                        await slot.Opened.Task;

                        if (slot.OpenError != null)
                            throw new InvalidOperationException(
                                $"Opening PopupType.{popupType} failed in a concurrent call.", slot.OpenError);

                        break;
                    }

                    case PopupState.Opened:
                    {
                        TPopup popup = Cast<TPopup>(slot.Popup);
                        await slot.RunExclusive(() => ActivateAsync(popup, payload));
                        return popup;
                    }

                    case PopupState.Closing:
                    {
                        await slot.Closed.Task;
                        break;
                    }
                }
            }
        }

        private async UniTask<TPopup> CreateAndOpen<TPopup, TPayload>(PopupType popupType,
            PopupEntryBase entry, TPayload payload)
            where TPopup : BasePopup
            where TPayload : IPopupPayload
        {
            PopupSlot slot = new PopupSlot(popupType);
            _slots[popupType] = slot;

            TPopup popup = null;
            try
            {
                popup = await _factory.Create<TPopup>(entry);
                slot.Popup = popup;

                PopupType declaredType = PopupTypeResolver.Get(popup.GetType());
                if (declaredType != popupType)
                    throw new InvalidOperationException(
                        $"{popup.GetType().Name} declares PopupType.{declaredType} via [{nameof(PopupTypeAttribute)}], " +
                        $"but it was opened as PopupType.{popupType}.");

                popup.SetPopupType(popupType);
                popup.SetCloseHandler(HandleCloseExternal);
                popup.NotifyOpening();

                await ActivateAsync(popup, payload);
                await popup.Show();

                slot.State = PopupState.Opened;
                slot.Opened.TrySetResult(popup);
            }
            catch (Exception e)
            {
                _slots.Remove(popupType);
                if (popup != null)
                    _factory.Release(popup);

                slot.OpenError = e;
                slot.Opened.TrySetResult(null);
                throw;
            }

            RaiseEvent(PopupOpened, popup);
            return popup;
        }

        private static UniTask ActivateAsync<TPayload>(BasePopup popup, TPayload payload)
            where TPayload : IPopupPayload
        {
            if (popup.PayloadType != typeof(TPayload))
                throw new InvalidOperationException(
                    $"Popup '{popup.GetType().Name}' expects payload '{popup.PayloadType.Name}', " +
                    $"but '{typeof(TPayload).Name}' was passed.");

            return popup is BasePopupT<TPayload> typed
                ? typed.Activate(payload)
                : UniTask.CompletedTask;
        }

        #endregion

        #region Close

        public async UniTask Close(PopupType popupType)
        {
            while (_slots.TryGetValue(popupType, out PopupSlot slot))
            {
                switch (slot.State)
                {
                    case PopupState.Opening:
                        await slot.Opened.Task;
                        break;

                    case PopupState.Opened:
                        await CloseSlot(slot);
                        return;

                    case PopupState.Closing:
                        await slot.Closed.Task;
                        return;
                }
            }
        }

        public UniTask Close<TPopup>() where TPopup : BasePopup
        {
            PopupType popupType = PopupTypeResolver.Get<TPopup>();

            return popupType != PopupType.None
                ? Close(popupType)
                : UniTask.CompletedTask;
        }

        public async UniTask CloseAll(params PopupType[] except)
        {
            for (int pass = 0; pass < MaxCloseAllPasses; pass++)
            {
                List<UniTask> tasks = new();

                foreach (PopupType popupType in new List<PopupType>(_slots.Keys))
                {
                    if (except != null && Array.IndexOf(except, popupType) >= 0)
                        continue;

                    tasks.Add(CloseSafe(popupType));
                }

                if (tasks.Count == 0)
                    return;

                await UniTask.WhenAll(tasks);
            }

            Debug.LogWarning($"[{nameof(PopupSystem)}] CloseAll: popups are still being opened after " +
                             $"{MaxCloseAllPasses} passes. Something keeps opening popups while closing.");
        }


        private async UniTask CloseSafe(PopupType popupType)
        {
            try
            {
                await Close(popupType);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }


        private async UniTask CloseSlot(PopupSlot slot)
        {
            slot.State = PopupState.Closing;

            try
            {
                await slot.WaitIdle();

                await slot.Popup.Deactivate();
                await slot.Popup.Hide();
            }
            finally
            {
                _slots.Remove(slot.Type);

                slot.Popup.NotifyClosed();
                RaiseEvent(PopupClosed, slot.Popup);

                _factory.Release(slot.Popup);
                slot.Closed.TrySetResult();

                if (_slots.Count == 0)
                    RaiseEvent(AllClosed);
            }
        }

        private void HandleCloseExternal(PopupType popupType)
        {
            Close(popupType).Forget();
        }

        #endregion

        #region Preload

        public UniTask Preload(PopupType popupType)
        {
            EnsureInitialized();

            if (!_entries.TryGetValue(popupType, out PopupEntryBase entry))
                throw new InvalidOperationException($"No prefab configured for PopupType.{popupType}");

            return _factory.Preload(entry);
        }

        public void Unload(PopupType popupType)
        {
            EnsureInitialized();

            if (_entries.TryGetValue(popupType, out PopupEntryBase entry))
                _factory.Unload(entry);
        }
        
        public void UnloadAll(params PopupType[] except)
        {
            EnsureInitialized();

            foreach (KeyValuePair<PopupType, PopupEntryBase> pair in _entries)
            {
                if (except != null && Array.IndexOf(except, pair.Key) >= 0)
                    continue;

                if (_slots.ContainsKey(pair.Key))
                    continue;

                _factory.Unload(pair.Value);
            }
        }

        #endregion

        #region Registry

        private void RegisterEntry(PopupEntryBase entry)
        {
            if (entry == null)
                return;

            if (!PopupEntryValidator.TryValidate(entry, out string error))
            {
                Debug.LogError($"[{nameof(PopupSystem)}] {error}");
                return;
            }

            if (_entries.ContainsKey(entry.Type))
            {
                Debug.LogError(
                    $"[{nameof(PopupSystem)}] PopupType.{entry.Type} appears several times in the configuration.");
                return;
            }

            _entries[entry.Type] = entry;
        }

        private PopupType ResolvePopupType<TPopup>() where TPopup : BasePopup
        {
            EnsureInitialized();

            PopupType popupType = PopupTypeResolver.Get<TPopup>();
            if (popupType == PopupType.None)
                throw new InvalidOperationException(
                    $"{typeof(TPopup).Name} has no [{nameof(PopupTypeAttribute)}] (or it is PopupType.None).");

            if (!_entries.ContainsKey(popupType))
                throw new InvalidOperationException(
                    $"No prefab configured for PopupType.{popupType} ({typeof(TPopup).Name}).");

            return popupType;
        }

        private void EnsureInitialized()
        {
            if (!_initialized)
                throw new InvalidOperationException($"{nameof(PopupSystem)} is not initialized. Call Init() first.");
        }

        private static TPopup Cast<TPopup>(BasePopup popup) where TPopup : BasePopup
        {
            return popup as TPopup ??
                   throw new InvalidOperationException(
                       $"Popup '{popup.GetType().Name}' is not a {typeof(TPopup).Name}.");
        }

        #endregion

        #region Events

        private static void RaiseEvent(Action<BasePopup> handler, BasePopup popup)
        {
            try
            {
                handler?.Invoke(popup);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void RaiseEvent(Action handler)
        {
            try
            {
                handler?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        #endregion
    }
}