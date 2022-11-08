using Zenject;

public class LevelUIMediator : MediatorBase
{
    private LevelUIView _levelUIView;
    private readonly FigureSpawnerModel _figureSpawnerModel;
    private Timer _timer;
    
    public LevelUIMediator(SignalBus signalBus, FigureSpawnerModel figureSpawnerModel, Timer timer) : base(signalBus)
    {
        _figureSpawnerModel = figureSpawnerModel;
        _timer = timer;
    }

    protected override void InitView(OnViewEnableViewSignal signal)
    {
        if (signal.view is not LevelUIView view) return;
        _levelUIView = view;
        _figureSpawnerModel.OnFigureClick += FigureClick;
        _timer.OnChangeTimer += UpdateTimer;
            
        SignalBus.Subscribe<OnPlayViewSignal>(OnPlay);
        SignalBus.Subscribe<OnCloseViewSignal>(OnClose);
        SignalBus.Subscribe<OnLevelCompleteViewSignal>(OnLevelComplete);
        SignalBus.Subscribe<OnResetGameViewSignal>(OnResetGame);
        SignalBus.Subscribe<OnStartTimerViewSignal>(OnStartTimer);
        SignalBus.Subscribe<OnStopTimerViewSignal>(OnStopTimer);
    }

    protected override void DisposeView(OnViewDisableViewSignal signal)
    {
        if (signal.view is not LevelUIView) return;
        
        _figureSpawnerModel.OnFigureClick -= FigureClick;
        _timer.OnChangeTimer -= UpdateTimer;
        SignalBus.Unsubscribe<OnPlayViewSignal>(OnPlay);
        SignalBus.Unsubscribe<OnCloseViewSignal>(OnClose);
        SignalBus.Unsubscribe<OnLevelCompleteViewSignal>(OnLevelComplete);
        SignalBus.Unsubscribe<OnResetGameViewSignal>(OnResetGame);
        SignalBus.Unsubscribe<OnStartTimerViewSignal>(OnStartTimer);
        SignalBus.Unsubscribe<OnStopTimerViewSignal>(OnStopTimer);
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
        SignalBus.Fire<ResetGameCommandSignal>();
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

    private void OnResetGame()
    {
        SignalBus.Fire<ResetGameCommandSignal>();
    }

    private void OnStartTimer()
    {
        SignalBus.Fire<StartTimerCommandSignal>();
    }

    private void OnStopTimer()
    {
        SignalBus.Fire<StopTimerCommandSignal>();
    }

    private void UpdateTimer(string time)
    {
        _levelUIView.UpdateTimer(time);
    }
}
