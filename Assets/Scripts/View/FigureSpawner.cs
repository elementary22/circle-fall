using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using DG.Tweening;
using static LevelSettings;

public class FigureSpawner : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _figureContainer;
    [SerializeField] private float _minCircleScale;

    private PrefabSettings _prefabSettings;
    private LevelInfo _levelInfo;
    private Dictionary<int, Tween> _tweens;
    private Dictionary<string, ObjectPool<Figure>> _poolsDictionary;
    private Pool _objectPool;

    private float _maxCircleScale;
    private Vector2 _screenBounds;
    private int _id = 0;
    private float _levelSpeed;
    private CancellationTokenSource _cts;

    private const float ScaleDuration = 0.25f;
    private const float SpeedCoefficient = 10f;

    public Action<float, float> onFigureClick;

    public void Init(PrefabSettings prefabSettings, LevelInfo levelInfo)
    {
        _prefabSettings = prefabSettings;
        _levelInfo = levelInfo;
        _levelSpeed = _levelInfo.levelSpeed;

        _screenBounds = GetScreenBounds();
        _maxCircleScale = GetMaxScale();

        _tweens = new Dictionary<int, Tween>();
        _objectPool = new Pool(_figureContainer);

        CreateFigurePools();
    }

    private Vector2 GetScreenBounds()
    {
        var screenBounds = _mainCamera.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height, _mainCamera.transform.position.z));
        return screenBounds;
    }

    private void CreateFigurePools()
    {
        _poolsDictionary = new Dictionary<string, ObjectPool<Figure>>();
        foreach (var figure in _prefabSettings.FiguresPrefabs)
        {
            var figureType = figure.GetType().Name;
            var pool = _objectPool.GetObjectPool(figure);
            _poolsDictionary.Add(figureType, pool);
        }
    }

    private void CreateFigure()
    {
        var scale = UnityEngine.Random.Range(_minCircleScale, _maxCircleScale);
        var figure = GetFigure();

        figure.onMove += MoveFigure;
        figure.onClicked += OnFigureClicked;

        figure.Init(scale, GetFigureSpeed(scale), _screenBounds);
    }

    private Figure GetFigure()
    {
        var figureType =
            _prefabSettings.FiguresPrefabs[UnityEngine.Random.Range(0, _prefabSettings.FiguresPrefabs.Count)];
        var pool = _poolsDictionary[figureType.GetType().Name];
        var figure = pool.Get();
        AssignNewID(figure);
        return figure;
    }

    private void ReleaseFigure(Figure figure)
    {
        var pool = _poolsDictionary[figure.GetType().Name];
        pool.Release(figure);
    }

    private void AssignNewID(Figure figure)
    {
        figure.Id = _id;
        _id++;
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
        var speed = _levelSpeed / scale * SpeedCoefficient;
        return speed;
    }

    private void OnFigureClicked(Figure figure)
    {
        if (_tweens[figure.Id] == null)
            return;

        var tween = figure.transform.DOScale(new Vector3(0f, 0f, 0f), ScaleDuration);
        tween.onComplete += () => OnCompleteScale(figure, tween);
    }

    private void OnCompleteScale(Figure figure, Tween tween)
    {
        _tweens[figure.Id].Kill();
        onFigureClick?.Invoke(figure.Scale, _maxCircleScale);
        ReleaseFigure(figure);
        tween.onComplete = null;
    }

    public void StartSpawn()
    {
        _cts = new CancellationTokenSource();
        SpawnFigures();
    }
    private UniTask _task;
    public void StopSpawn()
    {
        _cts.Cancel();
        _task.Forget();
    }

    private void MoveFigure(Figure figure, float height)
    {
        var startPosition = figure.transform.position;
        var finishPosition = new Vector3(startPosition.x, -_screenBounds.y - height, startPosition.z);
        var tween = figure.transform.DOMove(finishPosition, figure.Speed).SetSpeedBased().SetEase(Ease.Linear);
        tween.OnComplete(() => ReleaseFigure(figure)).WithCancellation(_cts.Token);
        _tweens.Add(figure.Id, tween);
    }
    
    private void SpawnFigures()
    {
        CreateFigure();
        _task = SetSpawnDelay();
    }

    private async UniTask SetSpawnDelay()
    {
        var randomDelayTime = UnityEngine.Random.Range(_levelInfo.minTimeDelay, _levelInfo.maxTimeDelay);
        await UniTask.Delay(TimeSpan.FromSeconds(randomDelayTime), false, PlayerLoopTiming.Update, _cts.Token);
        SpawnFigures();
    }
}