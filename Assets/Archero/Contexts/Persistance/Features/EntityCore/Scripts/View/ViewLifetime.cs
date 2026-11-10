using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Persistance.Features.EntityCore
{
    public sealed class ViewLifetime : IDisposable
    {
        private GameObject _gameObject;

        public ViewLifetime(GameObject gameObject) => _gameObject = gameObject;

        public void Dispose()
        {
            if (_gameObject != null)
                Object.Destroy(_gameObject);

            _gameObject = null;
        }
    }
}
