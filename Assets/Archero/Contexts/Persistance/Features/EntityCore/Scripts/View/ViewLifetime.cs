using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Persistance.Features.EntityCore
{
    public sealed class ViewLifetime : IDisposable
    {
        private MonoEntity _view;

        public ViewLifetime(MonoEntity monoEntity) => _view = monoEntity;

        public void Dispose()
        {
            if (_view == null)
                return;

            _view.Cleanup();
            Object.Destroy(_view.gameObject);
            _view = null;
        }
    }
}
