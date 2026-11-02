using System;
using Persistance.Features.User.Features.JsonUser;
using Template.Features.DataStorage;

namespace Persistance.Features.LevelProgression
{
    public interface ILevelProgressionService : IDataReader<PlayerData>, IDataWriter<PlayerData>, IDisposable
    {
        public bool IsLevelCompleted(int level);
        public void AddLevelToCompleted(int level);
        public bool CanPlayLevel(int level);
        void Init();
    }
}