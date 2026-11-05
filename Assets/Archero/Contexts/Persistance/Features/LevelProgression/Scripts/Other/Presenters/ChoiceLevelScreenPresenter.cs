using System.Collections.Generic;
using Contexts.Gameplay;
using Cysharp.Threading.Tasks;
using Template.Features.Config.Local;
using Template.Features.GameStateMachine;
using Template.Features.Presenters;
using Template.Features.ScreenNavigator;
using Template.Gameplay;

namespace Persistance.Features.LevelProgression
{
    public class ChoiceLevelScreenPresenter : IScreenPresenter<ChoiceLevelPayload>
    {
        private const string TitleName = "Levels";

        private readonly ILocalConfigService _localConfigService;
        private readonly IChoiceLevelScreenView _choiceLevelScreenView;
        private readonly ILevelProgressionService _levelProgressionService;
        private readonly IGameStateMachine _gameStateMachine;
        private readonly IScreenNavigator _screenNavigator;

        private LevelsListConfig _levelsListConfig;

        private readonly List<LevelTileView> _tileViews = new();
        private readonly List<LevelTilePresenter> _levelTilePresenters = new();
        
        private bool _isLevelStarting;
        private bool _disposed;

        public ChoiceLevelScreenPresenter(ILocalConfigService localConfigService,
            IChoiceLevelScreenView choiceLevelScreenView, ILevelProgressionService levelProgressionService,
            IGameStateMachine gameStateMachine, IScreenNavigator screenNavigator)
        {
            _localConfigService = localConfigService;
            _choiceLevelScreenView = choiceLevelScreenView;
            _levelProgressionService = levelProgressionService;
            _gameStateMachine = gameStateMachine;
            _screenNavigator = screenNavigator;
        }

        public void Init()
        {
            _choiceLevelScreenView.SetTitle(TitleName);
            
            _choiceLevelScreenView.BackClicked += OnBackClicked;

            _levelsListConfig = _localConfigService.GetConfig<LevelsListConfig>();

            for (int i = 0; i < _levelsListConfig.LevelConfigs.Count; i++)
            {
                LevelTileView tile = _choiceLevelScreenView.LevelTilesListView.Create();

                _tileViews.Add(tile);

                LevelTilePresenter levelTilePresenter =
                    new LevelTilePresenter(_levelProgressionService, tile, i + 1);

                levelTilePresenter.Init();
                _levelTilePresenters.Add(levelTilePresenter);
                levelTilePresenter.LevelSelected += OnLevelSelected;
            }
        }

        public UniTask Activate(ChoiceLevelPayload payload)
        {
            return UniTask.CompletedTask;
        }

        public void Dispose()
        {
            _choiceLevelScreenView.BackClicked -= OnBackClicked;
            
            foreach (LevelTilePresenter tilePresenter in _levelTilePresenters)
            {
                tilePresenter?.Dispose();
            }

            _choiceLevelScreenView.LevelTilesListView.Clear();
            _levelTilePresenters.Clear();
        }
        
        private void OnBackClicked()
        {
            _screenNavigator.Back().Forget();
        }
        
        private void OnLevelSelected(int level)
        {
            if (_isLevelStarting)
                return;

            _isLevelStarting = true;
            _choiceLevelScreenView.SetInputBlocked(true);
            
            foreach (LevelTilePresenter presenter in _levelTilePresenters)
                presenter.LevelSelected -= OnLevelSelected;


            StartLevelAsync(level);
        }
        
        private void StartLevelAsync(int level)
        {
            try
            {
               _gameStateMachine.Enter<GameplayState, GamePlayStatePayloads>(new(level));
            }
            catch
            {
                if (!_disposed)
                    Unblock();
                throw;
            }
        }

        private void Unblock()
        {
            _isLevelStarting = false;
            _choiceLevelScreenView.SetInputBlocked(false);
        }
    }
}