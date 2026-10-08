using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using UnityEngine;

namespace Template.Features.GameStateMachine
{
    public class GameStateMachine : IGameStateMachine, IDisposable
    {
        private readonly IClassFactory _classFactory;
        private readonly Queue<Func<UniTask>> _requests = new();
 
        private IExitableState _currentState;
        private CancellationTokenSource _stateLifetime;
 
        private bool _isPumping;
        private bool _isRecoveringFromFailure;
        private bool _isDisposed;
 
        public GameStateMachine(IClassFactory classFactory)
        {
            _classFactory = classFactory;
        }
 
        public Type CurrentStateType => _currentState?.GetType();
        public bool IsTransitioning => _isPumping;
 
        public void Enter<TState>() where TState : class, IState
        {
            Enqueue(() => SwitchState<TState>((state, lifetime) => state.Enter(lifetime)));
        }
 
        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload>
        {
            Enqueue(() => SwitchState<TState>((state, lifetime) => state.Enter(payload, lifetime)));
        }
 
        private void Enqueue(Func<UniTask> request)
        {
            if (_isDisposed)
            {
                return;
            }
 
            _requests.Enqueue(request);
            Pump().Forget();
        }
 
        private async UniTaskVoid Pump()
        {
            if (_isPumping)
            {
                return;
            }
 
            _isPumping = true;
 
            try
            {
                while (_requests.Count > 0 && !_isDisposed)
                {
                    Func<UniTask> request = _requests.Dequeue();
 
                    try
                    {
                        await request();
                    }
                    catch (OperationCanceledException)
                    {
                        _requests.Clear();
                    }
                    catch (Exception exception)
                    {
                        HandleFailure(exception);
                    }
                }
            }
            finally
            {
                _isPumping = false;
            }
        }
 
        private async UniTask SwitchState<TState>(Func<TState, CancellationToken, UniTask> enter)
            where TState : class, IExitableState
        {
            await ExitCurrentState();
 
            TState state = _classFactory.Create<TState>();
 
            _stateLifetime = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);
            _currentState = state;
            
            await enter(state, _stateLifetime.Token);
 
            _isRecoveringFromFailure = false;
        }
 
        private async UniTask ExitCurrentState()
        {
            if (_currentState == null)
            {
                return;
            }
 
            IExitableState state = _currentState;
            CancellationTokenSource lifetime = _stateLifetime;
 
            _currentState = null;
            _stateLifetime = null;
 
            try
            {
                lifetime?.Cancel();
                await state.Exit();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                lifetime?.Dispose();
            }
        }
 
        private void HandleFailure(Exception exception)
        {
            Debug.LogException(exception);
            
            _requests.Clear();
 
            if (_isRecoveringFromFailure)
            {
                Debug.LogError("[GameFlow] Failure while entering ErrorState. State machine is halted.");
                return;
            }
 
            _isRecoveringFromFailure = true;
            _requests.Enqueue(() => SwitchState<ErrorState>((state, lifetime) => state.Enter(exception, lifetime)));
        }
 
        public void Dispose()
        {
            _isDisposed = true;
            _requests.Clear();
 
            try
            {
                _stateLifetime?.Cancel();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
 
            _stateLifetime?.Dispose();
            _stateLifetime = null;
        }

    }
}