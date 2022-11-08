public class StartTimerCommand : ICommand
{
    private readonly Timer _timer;

    public StartTimerCommand(Timer timer)
    {
        _timer = timer;
    }

    public void Execute()
    {
        StartTimer();
    }

    private void StartTimer()
    {
        _timer.StartTimer();
    }
}

public class StartTimerCommandSignal
{
    
}
