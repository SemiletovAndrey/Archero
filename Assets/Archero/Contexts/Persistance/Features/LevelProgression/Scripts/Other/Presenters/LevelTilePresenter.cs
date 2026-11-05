using System;
using Contexts.Gameplay;
using Template.Features.GameStateMachine;
using Template.Features.Presenters;
using Template.Gameplay;
using UnityEngine;

namespace Persistance.Features.LevelProgression
{
    public class LevelTilePresenter : IDefaultClassPresenter
    {
        public event Action<int> LevelSelected;

        private readonly ILevelProgressionService _levelProgressionService;

        private readonly int _levelNumber;

        private readonly LevelTileView _levelTileView;

        public LevelTilePresenter(ILevelProgressionService levelProgressionService,
            LevelTileView levelTileView, int levelNumber)
        {
            _levelProgressionService = levelProgressionService;
            _levelTileView = levelTileView;
            _levelNumber = levelNumber;
        }

        public void Init()
        {
            _levelTileView.SetLevel(_levelNumber.ToString());

            if (_levelProgressionService.CanPlayLevel(_levelNumber))
            {
                _levelTileView.Clicked += OnClicked;
                if (_levelProgressionService.IsLevelCompleted(_levelNumber))
                {
                    _levelTileView.SetCompleted();
                }
                else
                {
                    _levelTileView.SetActiveColor();
                }
            }
            else
            {
                _levelTileView.SetBlock();
            }
        }

        public void Dispose()
        {
            _levelTileView.Clicked -= OnClicked;
        }

        private void OnClicked() => LevelSelected?.Invoke(_levelNumber);
    }
}