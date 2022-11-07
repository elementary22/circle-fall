using Zenject;

public class GameCompleteCommand : ICommand
{
    private readonly LevelUIMediator _levelUIMediator;

    public GameCompleteCommand(LevelUIMediator levelUIMediator, SignalBus signalBus)
    {
        _levelUIMediator = levelUIMediator;
    }

    public void Execute()
    {
        _levelUIMediator.OnGameComplete();
    }
}

public class GameCompleteCommandSignal : ISignal
{
    
}