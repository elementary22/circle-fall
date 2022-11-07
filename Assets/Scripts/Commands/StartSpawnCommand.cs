using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using static LevelSettings;

public class StartSpawnCommand : ICommand
{
    private readonly PrefabSettings _prefabSettings;
    private readonly FigureSpawnerModel _figureSpawnerModel;
    private LevelInfo _levelInfo;
    
    private const float ScaleDuration = 0.25f;
    private const float SpeedCoefficient = 10f;
    private int _id;
    private float _maxCircleScale;
    private Vector2 _screenBounds;
    
    public StartSpawnCommand(PrefabSettings prefabSettings, LevelSettings levelSettings, FigureSpawnerModel figureSpawnerModel)
    {
        _prefabSettings = prefabSettings;
        _figureSpawnerModel = figureSpawnerModel;
        _levelInfo = levelSettings.GetLevelInfo(_figureSpawnerModel.GameLevel);
        _screenBounds = GetScreenBounds(Camera.main);
        _maxCircleScale = GetMaxScale();
    }

    public void Execute()
    {
        _figureSpawnerModel.SpawnerTokenSource = new CancellationTokenSource();
        SpawnFigures().Forget();
    }

    private Vector2 GetScreenBounds(Camera camera)
    {
        var screenBounds = camera.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height, camera.transform.position.z));
        return screenBounds;
    }

    private async UniTaskVoid SpawnFigures()
    {
        while (true)
        {
            CreateFigure();
            var randomDelayTime = UnityEngine.Random.Range(_levelInfo.minTimeDelay, _levelInfo.maxTimeDelay);
            await UniTask.Delay(TimeSpan.FromSeconds(randomDelayTime), false, PlayerLoopTiming.Update, _figureSpawnerModel.SpawnerTokenSource.Token);
        }
    }

    private void CreateFigure()
    {
        var scale = UnityEngine.Random.Range(Config.MinCircleScale, _maxCircleScale);
        var figure = GetFigure();
        figure.onMove += MoveFigure;
        figure.onClicked += OnFigureClicked;

        figure.Init(scale, GetFigureSpeed(scale), _screenBounds);
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

    private Figure GetFigure()
    {
        var figureType =
            _prefabSettings.FiguresPrefabs[UnityEngine.Random.Range(0, _prefabSettings.FiguresPrefabs.Count)];
        var pool = _figureSpawnerModel.PoolsDictionary[figureType.GetType().Name];
        var figure = pool.Get();
        figure.Id = _id;
        _id++;
        _figureSpawnerModel.ActiveFigures.Add(figure);
        return figure;
    }

    private void ReleaseFigure(Figure figure)
    {
        var pool = _figureSpawnerModel.PoolsDictionary[figure.GetType().Name];
        var activeFigure = _figureSpawnerModel.ActiveFigures.Find(item => item.Id == figure.Id);
        _figureSpawnerModel.ActiveFigures.Remove(activeFigure);
        pool.Release(figure);
    }

    private void OnCompleteScale(Figure figure, Tween tween)
    {
        
        ReleaseFigure(figure);
        tween.onComplete = null;
        _figureSpawnerModel.OnFigureClick?.Invoke(figure.Scale, _maxCircleScale);
    }

    private float GetFigureSpeed(float scale)
    {
        var speed = _levelInfo.levelSpeed / scale * SpeedCoefficient;
        return speed;
    }

    private void OnFigureClicked(Figure figure)
    {
        if (_figureSpawnerModel.Tweens[figure.Id] == null)
            return;
        _figureSpawnerModel.Tweens[figure.Id].Kill();
        _figureSpawnerModel.Tweens.Remove(figure.Id);
        var tween = figure.transform.DOScale(Vector3.zero, ScaleDuration);
        tween.OnComplete(() => OnCompleteScale(figure, tween)).WithCancellation(_figureSpawnerModel.SpawnerTokenSource.Token);
    }
    
    private void MoveFigure(Figure figure, float height)
    {
        var startPosition = figure.transform.position;
        var finishPosition = new Vector3(startPosition.x, -_screenBounds.y - height, startPosition.z);
        var tween = figure.transform.DOMove(finishPosition, figure.Speed).SetSpeedBased().SetEase(Ease.Linear);
        tween.OnComplete(() => ReleaseFigure(figure)).WithCancellation(_figureSpawnerModel.SpawnerTokenSource.Token);
        _figureSpawnerModel.Tweens.Add(figure.Id, tween);
    }
}

public class StartSpawnCommandSignal
{

}