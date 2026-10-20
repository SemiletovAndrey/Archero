using Template.Features.NewInputFeature;
using UnityEngine;

namespace _Archero.Template.Features.NewInputFeature.Scripts.Example
{
    public class PlayerMovementFeature : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;

        private IInputBus _bus;
        private Vector2 _currentInput;

        /*public void Initialize(IInputBus bus)
        {
            _bus = bus;
            _bus.Subscribe<MovementSignal>(OnMovementReceived);
        }

        private void OnDestroy()
        {
            _bus?.Unsubscribe<MovementSignal>(OnMovementReceived);
        }

        private void OnMovementReceived(MovementSignal signal)
        {
            _currentInput = signal.Value;
        }

        private void Update()
        {
            if (_currentInput == Vector2.zero) return;

            Vector3 direction = new Vector3(_currentInput.x, 0f, _currentInput.y);
            transform.Translate(direction * (_moveSpeed * Time.deltaTime), Space.World);
        }*/
    }
}