using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.Log;

namespace Template.Features.Ping
{
    public class PingService : IPingService, IDisposable
    {
        private readonly IDebugLogger _logger;
        private readonly List<IPingProvider> _providers = new();

        private ConnectionState _state = ConnectionState.Unknown;
        private CancellationTokenSource _cts;
        private bool _isChecking;

        public PingService(IDebugLogger logger)
        {
            _logger = logger;
        }

        public event Action OnInternetRestored;
        public event Action OnInternetLost;
        public event Action<ConnectionState> OnStateChanged;

        public async UniTask InitAsync(IPingProvider defaultProvider)
        {
            _cts = new CancellationTokenSource();
            _providers.Add(defaultProvider);

            _ = CheckLoop(_cts.Token);
            await UniTask.Yield();
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public bool HasConnection()
        {
            return _state == ConnectionState.Connected;
        }

        public async UniTask WaitForOnline()
        {
            if (HasConnection())
            {
                return;
            }

            using var linked = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token);
            
            await UniTask.WaitUntil(
                () => _state == ConnectionState.Connected,
                cancellationToken: linked.Token);
        }

        public void AddProvider(IPingProvider provider)
        {
            if (!_providers.Contains(provider))
                _providers.Add(provider);
        }

        public void RemoveProvider(IPingProvider provider)
        {
            _providers.Remove(provider);
        }

        private async UniTaskVoid CheckLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_isChecking)
                {
                    await UniTask.Yield();
                    continue;
                }

                _isChecking = true;

                try
                {
                    var newState = await CheckProviders(token);

                    if (newState != _state)
                    {
                        var old = _state;
                        _state = newState;

                        OnStateChanged?.Invoke(newState);

                        if (old != ConnectionState.Connected && newState == ConnectionState.Connected)
                            OnInternetRestored?.Invoke();

                        if (old == ConnectionState.Connected && newState == ConnectionState.Disconnected)
                            OnInternetLost?.Invoke();
                    }
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    _logger.LogWarning($"PingService error: {e}");
                }
                finally
                {
                    _isChecking = false;
                }

                await UniTask.Delay(
                    TimeSpan.FromSeconds(PingConstants.CheckInternetInterval),
                    cancellationToken: token);
            }
        }

        private async UniTask<ConnectionState> CheckProviders(CancellationToken token)
        {
            PingResult result = await SafeCheck(_providers[^1], token);
            
            return result == PingResult.Success
                ? ConnectionState.Connected
                : ConnectionState.Disconnected;
        }

        private async UniTask<PingResult> SafeCheck(IPingProvider provider, CancellationToken token)
        {
            try
            {
                return await provider
                    .CheckConnection()
                    .Timeout(TimeSpan.FromSeconds(PingConstants.SlowConnectionThreshold))
                    .AttachExternalCancellation(token);
            }
            catch
            {
                return PingResult.Failed;
            }
        }
    }
}