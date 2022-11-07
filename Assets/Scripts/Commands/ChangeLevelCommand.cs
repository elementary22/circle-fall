
public class ChangeLevelCommand : ICommand
{
    private readonly LevelSettings _levelSettings;
    private readonly LevelUIMediator _levelUIMediator;
    private readonly FigureSpawnerModel _figureSpawnerModel;

    public ChangeLevelCommand(LevelSettings levelSettings, LevelUIMediator levelUIMediator, FigureSpawnerModel figureSpawnerModel)
    {
        _levelSettings = levelSettings;
        _levelUIMediator = levelUIMediator;
        _figureSpawnerModel = figureSpawnerModel;
    }

    public void Execute()
    {
        ChangeLevel();
    }

    private void ChangeLevel()
    {
        _levelUIMediator.OnUpdateLevelInfo(_levelSettings.GetLevelInfo(_figureSpawnerModel.GameLevel));
        _levelUIMediator.OnBackgroundChange();
        _levelUIMediator.OnStartLevelAnimation();
    }
}

public class ChangeLevelCommandSignal : ISignal
{
    
}
