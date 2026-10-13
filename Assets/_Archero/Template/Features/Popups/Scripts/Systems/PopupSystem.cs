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

            public readonly UniTaskCompletionSource<BasePopup> Opened = new();

            public readonly UniTaskCompletionSource Closed = new();

            public PopupSlot(PopupType type)
            {
                Type = type;
            }
        }

        private readonly IPopupFactory _factory;
        private readonly ILocalConfigService _localConfigService;

        private readonly Dictionary<PopupType, PopupEntryBase> _entries = new();
        private readonly Dictionary<PopupType, PopupSlot> _slots = new();

        private bool _initialized;

        public PopupSystem(ILocalConfigService localConfigService, IPopupFactory factory)
        {
            _localConfigService = localConfigService;
            _factory = factory;
        }

        public void Init()
        {
            _entries.Clear();

            PopupConfig config = _localConfigService.GetConfig<PopupConfig>();
            foreach (var entry in config.Entries)
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

        public UniTask<TPopup> Open<TPopup>() where TPopup : BasePopup<EmptyPopupPayload>
        {
            return OpenInternal<TPopup, EmptyPopupPayload>(ResolvePopupType<TPopup>(), default);
        }

        public UniTask<TPopup> Open<TPopup, TPayload>(TPayload payload)
            where TPopup : BasePopup<TPayload>
            where TPayload : IPopupPayload
        {
            return OpenInternal<TPopup, TPayload>(ResolvePopupType<TPopup>(), payload);
        }

        private async UniTask<TPopup> OpenInternal<TPopup, TPayload>(PopupType popupType, TPayload payload)
            where TPopup : BasePopup
            where TPayload : IPopupPayload
        {
            EnsureInitialized();

            if (!_entries.TryGetValue(popupType, out var entry))
                throw new InvalidOperationException($"No prefab configured for PopupType.{popupType}");

            while (true)
            {
                if (!_slots.TryGetValue(popupType, out var slot))
                    return await CreateAndOpen<TPopup, TPayload>(popupType, entry, payload);

                switch (slot.State)
                {
                    case PopupState.Opening:
                        await slot.Opened.Task;
                        break;

                    case PopupState.Opened:
                        var popup = Cast<TPopup>(slot.Popup);
                        await ActivateAsync(popup, payload);
                        return popup;

                    case PopupState.Closing:
                        await slot.Closed.Task;
                        break;
                }
            }
        }

        private async UniTask<TPopup> CreateAndOpen<TPopup, TPayload>(PopupType popupType, PopupEntryBase entry,
            TPayload payload)
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

                popup.PopupType = popupType;
                popup.SetCloseHandler(HandleCloseExternal);

                await ActivateAsync(popup, payload);
                await popup.Show();

                slot.State = PopupState.Opened;
                slot.Opened.TrySetResult(popup);
                return popup;
            }
            catch
            {
                _slots.Remove(popupType);
                if (popup != null)
                    _factory.Destroy(popup);

                slot.Opened.TrySetResult(null);
                throw;
            }
        }

        private static UniTask ActivateAsync<TPayload>(BasePopup popup, TPayload payload)
            where TPayload : IPopupPayload
        {
            if (popup.PayloadType != typeof(TPayload))
                throw new InvalidOperationException(
                    $"Popup '{popup.GetType().Name}' expects payload '{popup.PayloadType.Name}', " +
                    $"but '{typeof(TPayload).Name}' was passed.");

            return popup is BasePopup<TPayload> typed
                ? typed.Activate(payload)
                : UniTask.CompletedTask;
        }

        #endregion

        #region Close

        public async UniTask Close(PopupType popupType)
        {
            while (_slots.TryGetValue(popupType, out var slot))
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

        private async UniTask CloseSlot(PopupSlot slot)
        {
            slot.State = PopupState.Closing;

            try
            {
                await slot.Popup.Deactivate();
                await slot.Popup.Hide();
            }
            finally
            {
                _slots.Remove(slot.Type);
                _factory.Destroy(slot.Popup);
                slot.Closed.TrySetResult();
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

            if (!_entries.TryGetValue(popupType, out var entry))
                throw new InvalidOperationException($"No prefab configured for PopupType.{popupType}");

            return _factory.Preload(entry);
        }

        public void Unload(PopupType popupType)
        {
            EnsureInitialized();

            if (_entries.TryGetValue(popupType, out var entry))
                _factory.Unload(entry);
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

            if (!entry.HasSource)
            {
                Debug.LogError($"[{nameof(PopupSystem)}] PopupType.{entry.Type} doesn't have a prefab assigned.");
                return;
            }

            if (_entries.ContainsKey(entry.Type))
            {
                Debug.LogError($"[{nameof(PopupSystem)}] PopupType.{entry.Type} appears several times in the configuration.");
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
    }
}