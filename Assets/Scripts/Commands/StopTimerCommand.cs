public class StopTimerCommand : ICommand
{
    private readonly Timer _timer;

    public StopTimerCommand(Timer timer)
    {
        _timer = timer;
    }

    public void Execute()
    {
        StopTimer();
    }
    
    private void StopTimer()
    {
        _timer.StopTimer();
    }
}

public class StopTimerCommandSignal
{
    
}
