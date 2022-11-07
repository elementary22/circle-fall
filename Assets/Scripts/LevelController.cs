using UnityEngine;
using Zenject;
using static LevelSettings;

public class LevelController : IInitializable
{
    private LevelSettings _levelSettings;
    private LevelUIView _levelUIView;
    private LevelInfo _levelInfo;
    private SignalBus _signalBus;
    private FigureSpawnerModel _figureSpawnerModel;
    
    public LevelController(LevelSettings levelSettings, LevelUIView levelUIView, SignalBus signalBus, FigureSpawnerModel figureSpawnerModel)
    {
        _levelSettings = levelSettings;
        _levelUIView = levelUIView;
        _signalBus = signalBus;
        _figureSpawnerModel = figureSpawnerModel;
    }

    public void Initialize()
    {
        _levelInfo = _levelSettings.GetLevelInfo(_figureSpawnerModel.GameLevel);
        _levelUIView.Init(_levelInfo);
        _signalBus.Fire<OnApplicationLoadedCommandSignal>();
    }
}