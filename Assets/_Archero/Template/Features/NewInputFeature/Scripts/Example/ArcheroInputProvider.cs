using System;
using Template.Features.NewInputFeature;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Archero.Template.Features.NewInputFeature.Scripts.Example
{
    public class ArcheroInputProvider //: IInputProvider, IDisposable
    {
        /*private readonly IInputBus _bus;
        private readonly InputAction _moveAction;
        private readonly InputAction _attackAction;

        public ArcheroInputProvider(IInputBus bus, InputActionAsset actionsAsset)
        {
            _bus = bus;

            // Находим нужные Actions в контроллере
            var map = actionsAsset.FindActionMap("Player");
            _moveAction = map.FindAction("Move");
            _attackAction = map.FindAction("Attack");

            BindEvents();
        }

        private void BindEvents()
        {
            // Слушаем изменение оси джойстика
            _moveAction.performed += OnMoveChanged;
            _moveAction.canceled += OnMoveCanceled;

            // Слушаем кнопки
            _attackAction.started += _ => _bus.Publish(new PrimaryActionSignal(true));
            _attackAction.canceled += _ => _bus.Publish(new PrimaryActionSignal(false));
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
            _moveAction.Enable();
            _attackAction.Enable();
        }

        public void Disable()
        {
            _moveAction.Disable();
            _attackAction.Disable();
        }

        public void Dispose()
        {
            _moveAction.performed -= OnMoveChanged;
            _moveAction.canceled -= OnMoveCanceled;
            Disable();
        }*/
    }
}