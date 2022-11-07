
using UnityEngine;
using Zenject;

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
        _figureSpawnerModel.SpawnerTokenSource.Cancel();

        foreach (Transform child in _container)
        {
            var figure = child.GetComponent<Figure>();
            var pool = _figureSpawnerModel.PoolsDictionary[figure.GetType().Name];
            pool.Release(figure);
        }
    }
}

public class StopSpawnCommandSignal : ISignal
{
    
}