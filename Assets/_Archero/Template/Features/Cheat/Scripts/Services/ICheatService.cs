using System;
using System.Collections.Generic;

namespace Template.Features.Cheat
{
    public interface ICheatService
    {
        public event Action UpdatedCheatCollection; 
        
        public IReadOnlyList<CheatCommandDescriptor> GetCheatDescriptors();
        
        public void Execute(string input);
        
        public void Register(BaseCheatHandler handler);
        public void Unregister(BaseCheatHandler handler);
    }
}