public class OnViewEnableViewSignal : ISignal
{
    public IView view { get; }

    public OnViewEnableViewSignal(IView view)
    {
        this.view = view;
    }
}

public class OnViewDisableViewSignal : ISignal
{
    public IView view { get; }

    public OnViewDisableViewSignal(IView view)
    {
        this.view = view;
    }
}

public class OnPlayViewSignal : ISignal
{
    
}

public class OnCloseViewSignal : ISignal
{
    
}

public class OnLevelCompleteViewSignal : ISignal
{
    
}

public class OnResetGameViewSignal : ISignal
{
    
}