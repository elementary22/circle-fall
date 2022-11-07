using UnityEngine;
using UnityEngine.Pool;

public class Pool
{
    private readonly Transform _container;
    private readonly FigureFactory _factory;

    public Pool(Transform container, FigureFactory factory)
    {
        _container = container;
        _factory = factory;
    }

    public ObjectPool<Figure> GetObjectPool(Figure figure)
    {
        return new ObjectPool<Figure>(() =>
            {
                var newFigure = _factory.Create(figure);
                newFigure.transform.SetParent(_container, false);
                return newFigure;
            },
            item => item.GetFromPool(), item => item.ReturnToPool(), item => item.DestroyPoolObject(),
            false, 5, 10);
    }
}
