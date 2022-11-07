using Zenject;

public class LevelUIMediator : MediatorBase
{
    private LevelUIView _levelUIView;
    private readonly FigureSpawnerModel _figureSpawnerModel;
    
    public LevelUIMediator(SignalBus signalBus, FigureSpawnerModel figureSpawnerModel) : base(signalBus)
    {
        _figureSpawnerModel = figureSpawnerModel;
    }

    protected override void InitView(OnViewEnableViewSignal signal)
    {
        if (signal.view is not LevelUIView view) return;
        _levelUIView = view;
        _figureSpawnerModel.OnFigureClick += FigureClick;
            
        SignalBus.Subscribe<OnPlayViewSignal>(OnPlay);
        SignalBus.Subscribe<OnCloseViewSignal>(OnClose);
        SignalBus.Subscribe<OnLevelCompleteViewSignal>(OnLevelComplete);
        SignalBus.Subscribe<OnResetGameViewSignal>(OnResetGame);
    }

    protected override void DisposeView(OnViewDisableViewSignal signal)
    {
        if (signal.view is not LevelUIView) return;
        _figureSpawnerModel.OnFigureClick -= FigureClick;
        SignalBus.Unsubscribe<OnPlayViewSignal>(OnPlay);
        SignalBus.Unsubscribe<OnCloseViewSignal>(OnClose);
        SignalBus.Unsubscribe<OnLevelCompleteViewSignal>(OnLevelComplete);
        SignalBus.Unsubscribe<OnResetGameViewSignal>(OnResetGame);
    }

    private void FigureClick(float circleScale, float maxScale)
    {
        _levelUIView.UpdateScore(circleScale, maxScale);
    }

    private void OnPlay()
    {
        SignalBus.Fire<StartSpawnCommandSignal>();
    }

    private void OnClose()
    {
        SignalBus.Fire<StopSpawnCommandSignal>();
    }

    public void OnUpdateLevelInfo(LevelSettings.LevelInfo levelInfo)
    {
        _levelUIView.UpdateLevelInfo(levelInfo);
    }

    public void OnBackgroundChange()
    {
        _levelUIView.GetLevelBackground();
    }

    public void OnStartLevelAnimation()
    {
        _levelUIView.StartLevelAnimation();
    }
    
    private void OnLevelComplete()
    {
        SignalBus.Fire<CheckLevelCompleteCommandSignal>();
    }

    public void OnGameComplete()
    {
        _levelUIView.CompleteGame();
    }

    public void OnResetGame()
    {
        SignalBus.Fire<ResetGameCommandSignal>();
    }
}
