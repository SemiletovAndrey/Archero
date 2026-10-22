using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Template.Scripts.Helpers;
using UnityEngine;
using Zenject;

namespace Template.Features.Backend.Socket
{
    public abstract class BaseSocketHandler : IDisposable
    {
        private ISocketApiService _socketApiService;
        
        private ClientWebSocket _webSocket;
        private CancellationTokenSource _socketToken;
        private List<RegisteredSocketEvent> _registeredEvents = new();

        [Inject]
        private void Construct(ISocketApiService socketApiService)
        {
            _socketApiService = socketApiService;
        }

        protected virtual string RouteController => string.Empty;
        protected virtual string RouteVersion => BackendConstants.DefaultVersion;
        protected virtual bool WithPingPong => true;

        public bool IsSocketOpen => _webSocket?.State == WebSocketState.Open;

        public virtual UniTask InitAsync()
        {
            RegisterEvents();

            if (WithPingPong)
            {
                RegisterPingPong();
            }

            return UniTask.CompletedTask;
        }

        public virtual void Dispose()
        {
            Shutdown();
        }

        protected virtual void RegisterEvents()
        {
        }

        protected virtual void OnConnectionStatusChanged(bool status)
        {
        }

        protected virtual string GetUrl()
        {
            return BackendUtils.ConcatRoute(_socketApiService.GetBaseUrl(), RouteVersion, RouteController);
        }

        protected virtual UniTask Pong()
        {
            return SendMessage(PongSocketMessage.MessageName, new PongSocketMessage());
        }

        protected async UniTask<bool> ConnectAsync(params object[] extraRoute)
        {
            try
            {
                if (IsSocketOpen)
                {
                    OnConnectionStatusChanged(true);
                    return true;
                }

                _webSocket = new ClientWebSocket();
                _socketToken = new CancellationTokenSource();

                string route = BackendUtils.CreateFullRouteWithUri(GetUrl(), extraRoute);
                
                Debug.Log("Trying to connect to " + route);
                Uri uri = new(route);

                await _webSocket.ConnectAsync(uri, _socketToken.Token);

                Debug.Log("WebSocket connected successfully");
                OnConnectionStatusChanged(true);

                return true;
            }
            catch (Exception e)
            {
                Debug.Log($"WebSocket connection failed: {e.Message}");
                OnConnectionStatusChanged(false);
                return false;
            }
        }

        protected void Shutdown()
        {
            try
            {
                _socketToken?.Cancel();

                if (IsSocketOpen)
                {
                    var closeTask = _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect requested",
                        CancellationToken.None);
                    closeTask.Wait(1000);
                }

                _webSocket?.Dispose();
                _socketToken?.Dispose();

                _webSocket = null;

                OnConnectionStatusChanged(false);
                Debug.Log("WebSocket disconnected");
            }
            catch (Exception e)
            {
                Debug.Log($"Error during WebSocket disconnect: {e.Message}");
            }
        }

        protected async UniTask Listen()
        {
            byte[] buffer = new byte[4096];

            try
            {
                while (IsSocketOpen && !_socketToken.IsCancellationRequested)
                {
                    WebSocketReceiveResult result = await _webSocket.ReceiveAsync(buffer, _socketToken.Token);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        
                        Debug.Log($"Received message: {message}");
                        ProcessMessage(message);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Debug.Log("WebSocket closed by server");
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("WebSocket listening cancelled");
            }
            catch (Exception e)
            {
                Debug.Log($"WebSocket listening error: {e.Message}.\n{e.StackTrace}");
            }
            finally
            {
                OnConnectionStatusChanged(false);
            }
        }

        protected UniTask SendMessage(string eventName, object data)
        {
            SocketMessage message = new()
            {
                EventName = eventName,
                EventArgs = data
            };

            return SendMessage(message);
        }

        protected async UniTask SendMessage(SocketMessage message)
        {
            if (!IsSocketOpen)
            {
                Debug.Log("Cannot send message: WebSocket is not connected.");
                return;
            }

            try
            {
                byte[] bytesToSend = BuildMessage(message);

                try
                {
                    if (!_socketToken.IsCancellationRequested)
                    {
                        await _webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, _socketToken.Token);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"WebSocket sending error: {e.Message}");
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("WebSocket sending cancelled");
            }
            catch (Exception e)
            {
                Debug.LogError($"WebSocket sending error: {e.Message}.\n{e.StackTrace}");
            }
        }

        protected void RegisterEvent<T>(string eventName, Action<T> callback)
        {
            RegisteredSocketEvent<T> registeredEvent = new(eventName, callback);

            _registeredEvents.Add(registeredEvent);
            
            Debug.Log($"Registering event {registeredEvent.EventName}");
        }

        protected void UnregisterEvent(string eventKey)
        {
            int index = _registeredEvents.FindIndex(x => x.EventName == eventKey);

            if (index == -1)
            {
                return;
            }

            _registeredEvents.RemoveAt(index);
            
            Debug.Log($"Unregistering event {eventKey}");
        }

        private void OnMessageReceived(SocketEvent socketEvent)
        {
            RegisteredSocketEvent registeredEvent = _registeredEvents
                .FirstOrDefault(registeredEvent => registeredEvent.CheckEvent(socketEvent.EventName));

            if (registeredEvent == null)
            {
                Debug.LogWarning($"Event {socketEvent.EventName} was not found\nArgs: {socketEvent.EventArgs}");
                return;
            }

            registeredEvent.InvokeCallback(socketEvent.EventArgs);
        }

        private void ProcessMessage(string message)
        {
            try
            {
                var socketEvent = JsonHelpers.Deserialize<SocketEvent>(message);
                OnMessageReceived(socketEvent);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to process WebSocket message: {e.Message}.\n{e.StackTrace}");
            }
        }

        private byte[] BuildMessage<T>(T data)
        {
            string json = JsonHelpers.Serialize(data);
            Debug.Log($"Sending message: {json}");

            return Encoding.UTF8.GetBytes(json);
        }

        private void Ping(PingSocketEvent socketEvent)
        {
            Pong();
        }

        private void RegisterPingPong()
        {
            RegisterEvent<PingSocketEvent>(PingSocketEvent.EventName, Ping);
        }
    }
}