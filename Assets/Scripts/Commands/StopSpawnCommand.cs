public class StopSpawnCommand : ICommand
{
    private readonly FigureSpawnerModel _figureSpawnerModel;

    public StopSpawnCommand(FigureSpawnerModel figureSpawnerModel)
    {
        _figureSpawnerModel = figureSpawnerModel;
    }
    
    public void Execute()
    {
        _figureSpawnerModel.SpawnerTokenSource.Cancel();
        ReturnActiveFiguresToPool();
    }

    private void ReturnActiveFiguresToPool()
    {
        foreach (var figure in _figureSpawnerModel.ActiveFigures)
        {
            var pool = _figureSpawnerModel.PoolsDictionary[figure.GetType().Name];
            pool.Release(figure);
        }
        _figureSpawnerModel.ActiveFigures.Clear();
        _figureSpawnerModel.Tweens.Clear();
    }
}

public class StopSpawnCommandSignal : ISignal
{
    
}