using Zenject;

public class CheckLevelCompleteCommand : ICommand
{
    private readonly FigureSpawnerModel _figureSpawnerModel;
    private readonly SignalBus _signalBus;

    public CheckLevelCompleteCommand(FigureSpawnerModel figureSpawnerModel, SignalBus signalBus)
    {
        _figureSpawnerModel = figureSpawnerModel;
        _signalBus = signalBus;
    }

    public void Execute()
    {
        CompleteLevel();
    }
    
    private void CompleteLevel()
    {
        _signalBus.Fire<StopSpawnCommandSignal>();
        if (_figureSpawnerModel.GameLevel != Config.MaxLevel)
        {
            _figureSpawnerModel.GameLevel++;
            _signalBus.Fire<ChangeLevelCommandSignal>();
        }
        else _signalBus.Fire<GameCompleteCommandSignal>();
    }
}

public class CheckLevelCompleteCommandSignal : ISignal
{
    
}
