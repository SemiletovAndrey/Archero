using System;
using UnityEngine;
using Zenject;

namespace Template.Features.NewInputFeature.Scripts.Example
{
    public class PlayerMovementFeature : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;

        private IInputBus _bus;
        private Vector2 _currentInput;

        [Inject]
        private void Construct(IInputBus bus)
        {
            _bus = bus;
        }

        private void Start()
        {
            _bus.Subscribe<MovementSignal>(OnMovementReceived);
        }

        private void OnDestroy()
        {
            _bus?.Unsubscribe<MovementSignal>(OnMovementReceived);
        }

        private void OnMovementReceived(MovementSignal signal)
        {
            _currentInput = signal.Value;
            Debug.Log("Movement Received: " + signal.Value);
        }

        private void Update()
        {
            if (_currentInput == Vector2.zero) return;

            Vector3 direction = new Vector3(_currentInput.x, 0f, _currentInput.y);
            transform.Translate(direction * (_moveSpeed * Time.deltaTime), Space.World);
        }
    }
}