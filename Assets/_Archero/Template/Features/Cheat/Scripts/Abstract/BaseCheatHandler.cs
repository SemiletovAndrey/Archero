using System;
using Zenject;

namespace Template.Features.Cheat
{
    public abstract class BaseCheatHandler : IDisposable
    {
        protected ICheatService _cheatService;
        
        [Inject]
        private void Construct(ICheatService cheatService)
        {
            _cheatService = cheatService;
        }
        
        public void Init()
        {
            _cheatService.Register(this);
        }

        public void Dispose()
        {
            _cheatService.Unregister(this);
        }

        protected virtual void AddSubscriptions()
        {
                
        }

        protected virtual void RemoveSubscriptions()
        {
            
        }
    }
}