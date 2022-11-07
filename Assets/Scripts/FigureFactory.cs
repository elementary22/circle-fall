using UnityEngine;
using Zenject;

public class FigureFactory : IFactory<UnityEngine.Object, Figure>
{
    private readonly DiContainer _container;

    public FigureFactory(DiContainer container)
    {
        _container = container;
    }

    public Figure Create(Object obj)
    {
        return _container.InstantiatePrefabForComponent<Figure>(obj);
    }
}
