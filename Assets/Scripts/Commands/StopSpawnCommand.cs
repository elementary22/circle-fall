using UnityEngine;

public class StopSpawnCommand : ICommand
{
    private readonly FigureSpawnerModel _figureSpawnerModel;
    private Transform _container;

    public StopSpawnCommand(FigureSpawnerModel figureSpawnerModel, FigureContainerView container)
    {
        _figureSpawnerModel = figureSpawnerModel;
        _container = container.transform;
    }
    
    public void Execute()
    {
        foreach (Transform child in _container)
        {
            var figure = child.GetComponent<Figure>();
            var pool = _figureSpawnerModel.PoolsDictionary[figure.GetType().Name];
            pool.Release(figure);
        }
        
        _figureSpawnerModel.SpawnerTokenSource.Cancel();
    }
}

public class StopSpawnCommandSignal : ISignal
{
    
}