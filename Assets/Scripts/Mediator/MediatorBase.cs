using Zenject;

public abstract class MediatorBase : IInitializable, ILateDisposable
{
    protected readonly SignalBus SignalBus;

    protected MediatorBase(SignalBus signalBus)
    {
        SignalBus = signalBus;
    }
    
    public void Initialize()
    {
        SignalBus.Subscribe<OnViewEnableViewSignal>(InitView);
        SignalBus.Subscribe<OnViewDisableViewSignal>(DisposeView);
    }

    public void LateDispose()
    {
        SignalBus.Unsubscribe<OnViewEnableViewSignal>(InitView);
        SignalBus.Unsubscribe<OnViewDisableViewSignal>(DisposeView);
    }

    protected virtual void InitView(OnViewEnableViewSignal signal)
    {
        
    }

    protected virtual void DisposeView(OnViewDisableViewSignal signal)
    {
        
    }
}
