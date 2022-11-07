using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CreateFigureSpawnerCommand : ICommand
{
    private readonly FigureSpawnerModel _figureSpawnerModel;
    private readonly PrefabSettings _prefabSettings;
    private readonly FigureFactory _factory;
    private readonly FigureContainerView _container;
    

    public CreateFigureSpawnerCommand(FigureSpawnerModel figureSpawnerModel, PrefabSettings prefabSettings, FigureFactory factory, FigureContainerView containerView)
    {
        _figureSpawnerModel = figureSpawnerModel;
        _prefabSettings = prefabSettings;
        _factory = factory;
        _container = containerView;
    }

    public void Execute()
    {
        InitPool();
        CreateFigurePools();
    }

    private void InitPool()
    {
        var figureContainer = _container.transform;
        _figureSpawnerModel.PoolObject = new Pool(figureContainer, _factory);
    }
    
    private void CreateFigurePools()
    {
        _figureSpawnerModel.PoolsDictionary = new Dictionary<string, ObjectPool<Figure>>();
        foreach (var figure in _prefabSettings.FiguresPrefabs)
        {
            var figureType = figure.GetType().Name;
            var pool = _figureSpawnerModel.PoolObject.GetObjectPool(figure);
            _figureSpawnerModel.PoolsDictionary.Add(figureType, pool);
        }
    }
}

public class OnApplicationLoadedCommandSignal 
{

}
