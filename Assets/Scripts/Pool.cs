using UnityEngine;
using UnityEngine.Pool;

public class Pool
{
    private readonly PrefabSettings _prefabSettings;
    private readonly Transform _container;
    
    public Pool(PrefabSettings prefabSettings, Transform container)
    {
        _prefabSettings = prefabSettings;
        _container = container;
    }

    public ObjectPool<Figure> GetObjectPool(Figure figure)
    {
        return new ObjectPool<Figure>(() => Object.Instantiate(_prefabSettings.GetFigurePrefab(figure), _container, false),
            figure => figure.GetFromPool(), figure => figure.ReturnToPool(), figure => figure.DestroyPoolObject(),
            false, 5, 10);
    }
}
