using System.Collections.Generic;
using Persistance.Features.User.Features.JsonUser;
using Template.Features.DataStorage;

namespace Persistance.Features.LevelProgression
{
    public class LevelProgressionService : ILevelProgressionService
    {
        private const int FirstLevel = 1;

        private readonly List<int> _completedLevels = new List<int>();

        private IDataProvider<PlayerData> _playerDataProvider;
        
        public LevelProgressionService(IDataProvider<PlayerData> playerDataProvider)
        {
            _playerDataProvider = playerDataProvider;
        }
        
        public void Init()
        {
            _playerDataProvider.RegisterWriter(this);
            _playerDataProvider.RegisterReader(this);
        }
        
        public void Dispose()
        {
            _playerDataProvider.UnRegisterReader(this);
            _playerDataProvider.UnRegisterReader(this);
        }

        public bool IsLevelCompleted(int level)
        {
            return _completedLevels.Contains(level);
        }

        public void AddLevelToCompleted(int level)
        {
            if (IsLevelCompleted(level))
                return;

            _completedLevels.Add(level);
        }

        public bool CanPlayLevel(int level)
        {
            return level == FirstLevel ||  PreviousLevelIsCompleted(level);
        }

        public void ReadFrom(PlayerData data)
        {
            _completedLevels.Clear();
            _completedLevels.AddRange(data.CompletedLevels);
        }

        public void WriteTo(PlayerData data)
        {
            data.CompletedLevels.Clear();
            data.CompletedLevels.AddRange(_completedLevels);
        }

        private bool PreviousLevelIsCompleted(int level)
        {
            return IsLevelCompleted(level - 1);
        }
    }
}