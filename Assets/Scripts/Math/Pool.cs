using UnityEngine;
using UnityEngine.Pool;

public class Pool
{
    private readonly Transform _container;
    
    public Pool(Transform container)
    {
        _container = container;
    }

    public ObjectPool<Figure> GetObjectPool(Figure figure)
    {
        return new ObjectPool<Figure>(() => Object.Instantiate(figure, _container, false),
            item => item.GetFromPool(), item => item.ReturnToPool(), item => item.DestroyPoolObject(),
            false, 5, 10);
    }
}
