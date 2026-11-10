using System;
using Gameplay.Features.LifeCycle;
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
            _bus = bus;
            
            _actions = new GameInputActions();

            BindEvents();
            
            Enable();
        }
        
        public void Dispose()
        {
            _actions.Player.Move.performed -= OnMoveChanged;
            _actions.Player.Move.canceled -= OnMoveCanceled;
            
            _actions.Player.Attack.performed -= OnAttackPerformed;
            _actions.Player.Attack.canceled -= OnAttackCanceled;
            
            Disable();
            
            _actions.Dispose();
        }

        private void BindEvents()
        {
            _actions.Player.Move.performed += OnMoveChanged;
            _actions.Player.Move.canceled += OnMoveCanceled;
            
            _actions.Player.Attack.performed += OnAttackPerformed;
            _actions.Player.Attack.canceled += OnAttackCanceled;
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
        
        private void OnAttackPerformed(InputAction.CallbackContext ctx)
        {
            _bus.Publish(new AttackSignal(true));
        }

        private void OnAttackCanceled(InputAction.CallbackContext ctx)
        {
            _bus.Publish(new AttackSignal(false));
        }
    }
}