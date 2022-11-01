using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public class LevelView : MonoBehaviour
{
    [SerializeField] private Transform _figureContainer;
    [SerializeField] private float _minCircleScale;
    private PrefabSettings _prefabSettings;
    private float _maxCircleScale;
    private Vector2 _screenBounds;
    private Dictionary<int, Coroutine> _routines;
    private int _id = 0;
    private float _levelSpeed;
    private Pool _pool;
    private List<Figure> _figuresList;
    private Dictionary<string, ObjectPool<Figure>> _objectPools;

    public Action<float, float> onFigureClick;

    public void Init(PrefabSettings prefabSettings, float speed)
    {
        _prefabSettings = prefabSettings;
        _screenBounds =
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        _levelSpeed = speed;
        _maxCircleScale = GetMaxScale();
        _figuresList = _prefabSettings.GetFigures();
        _routines = new Dictionary<int, Coroutine>();
        _pool = new Pool(_prefabSettings, _figureContainer);
        CreateFigurePools();
    }

    private void CreateFigurePools()
    {
        _objectPools = new Dictionary<string, ObjectPool<Figure>>();
        foreach (var figure in _figuresList)
        {
            var figureType = figure.GetType().Name;
            var pool = _pool.GetObjectPool(figure);
            _objectPools.Add(figureType, pool);
        }
    }

    public void SpawnFigure()
    {
        var scale = UnityEngine.Random.Range(_minCircleScale, _maxCircleScale);
        var figure = GetFigure();

        figure.onMove += MoveFigure;
        figure.onClicked += OnFigureClicked;

        figure.Init(scale, GetFigureSpeed(scale), _screenBounds);
    }

    private Figure GetFigure()
    {
        var figureType = _figuresList[UnityEngine.Random.Range(0, _figuresList.Count)];
        var pool = _objectPools[figureType.GetType().Name];
        var figure = pool.Get();
        figure.Id = _id;
        _id++;
        return figure;
    }

    private void ReleaseFigure(Figure figure)
    {
        var pool = _objectPools[figure.GetType().Name];
        pool.Release(figure);
        figure.Dispose();
    }

    private void OnFigureClicked(Figure figure)
    {
        if (_routines[figure.Id] == null)
            return;

        onFigureClick?.Invoke(figure.Scale, _maxCircleScale);
        StopCoroutine(_routines[figure.Id]);
        ReleaseFigure(figure);
    }

    private float GetMaxScale()
    {
        float maxScale;
        if (_screenBounds.x > _screenBounds.y)
            maxScale = _screenBounds.y * 2;
        else
            maxScale = _screenBounds.x * 2;

        return maxScale;
    }

    private float GetFigureSpeed(float scale)
    {
        var speed = _levelSpeed / scale;
        return speed;
    }

    public void StopGame()
    {
        StopAllCoroutines();
        foreach (var pool in _objectPools)
            pool.Value.Clear();
    }

    private void MoveFigure(Figure figure, float height)
    {
        var startPosition = figure.transform.position;
        var finishPosition = new Vector3(startPosition.x, -_screenBounds.y - height, startPosition.z);
        var routine = StartCoroutine(Move(figure, startPosition, finishPosition, figure.Speed));

        _routines.Add(figure.Id, routine);
    }

    private IEnumerator Move(Figure figure, Vector3 startPoint, Vector3 finishPoint, float speed)
    {
        float step = 0;
        while (step < 1)
        {
            figure.transform.position = Vector3.Lerp(startPoint, finishPoint, step);
            step += speed * Time.deltaTime;
            yield return null;
        }
        figure.transform.position = finishPoint;
        ReleaseFigure(figure);
    }
}