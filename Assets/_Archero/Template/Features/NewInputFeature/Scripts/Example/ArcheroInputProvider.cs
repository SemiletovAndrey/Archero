using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Template.Features.NewInputFeature.Scripts.Example
{
    public class ArcheroInputProvider : IInputProvider, IDisposable
    {
        private readonly IInputBus _bus;
        private readonly GameInputActions _actions;

        public ArcheroInputProvider(IInputBus bus)
        {
            Debug.Log("ArcheroInputProvider");
            _bus = bus;
            
            _actions = new GameInputActions();

            BindEvents();
            
            Enable();
        }

        private void BindEvents()
        {
            _actions.Player.Move.performed += OnMoveChanged;
            _actions.Player.Move.canceled += OnMoveCanceled;
        }

        private void OnMoveChanged(InputAction.CallbackContext ctx)
        {
            Vector2 value = ctx.ReadValue<Vector2>();
            _bus.Publish(new MovementSignal(value));
        }

        private void OnMoveCanceled(InputAction.CallbackContext ctx)
        {
            _bus.Publish(new MovementSignal(Vector2.zero));
        }

        public void Enable()
        {
            _actions.Player.Enable();
        }

        public void Disable()
        {
            _actions.Player.Disable();
        }

        public void Dispose()
        {
            _actions.Player.Move.performed -= OnMoveChanged;
            _actions.Player.Move.canceled -= OnMoveCanceled;
            
            Disable();
            
            _actions.Dispose();
        }
    }
}