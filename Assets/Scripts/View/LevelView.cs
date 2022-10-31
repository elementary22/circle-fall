using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

public class LevelView : MonoBehaviour
{
    [SerializeField] private Transform _circleContainer;
    [SerializeField] private float _minCircleScale;
    private PrefabSettings _prefabSettings;
    private float _maxCircleScale;
    private Vector2 _screenBounds;
    private Dictionary<int, Coroutine> _routines;
    private Dictionary<TextureSize, Sprite> _sprites;
    private int _id = 0;
    private float _levelSpeed;
    private ObjectPool<Circle> _pool;

    public Action<float, float> onCircleClick;

    public void Init(PrefabSettings prefabSettings, Dictionary<TextureSize, Sprite> sprites, float speed)
    {
        _prefabSettings = prefabSettings;
        _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        _levelSpeed = speed;
        _maxCircleScale = GetMaxScale();

        _sprites = sprites;
        _routines = new Dictionary<int, Coroutine>();
        _pool = new ObjectPool<Circle>(CreateCircle, circle => circle.GetFromPool(), circle => circle.ReturnToPool(),
        circle => circle.DestroyPoolObject(), false, 5, 10);
    }
    
    private Circle CreateCircle()
    {
        var circle = Instantiate(_prefabSettings.GetFigurePrefab<Circle>(Figures.CIRCLE), _circleContainer, false);
        circle.onMove += MoveCircle;
        circle.onClicked += OnCircleClicked;
        return circle;
    }
    
    public void SpawnCircle()
    {
        var scale = UnityEngine.Random.Range(_minCircleScale, _maxCircleScale);
        var circle = _pool.Get();

        circle.Id = _id;
        circle.SetSize(scale);
        circle.SetSprite(GetSprite(scale));
        circle.SetSpeed(GetCircleSpeed(scale));
        circle.SetTransformPosition(_screenBounds);

        _id++;
    }

    private void OnCircleClicked(Circle circle)
    {
        if (_routines[circle.Id] == null)
            return;

        onCircleClick?.Invoke(circle.Scale, _maxCircleScale);
        StopCoroutine(_routines[circle.Id]);
        _pool.Release(circle);
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

    private float GetCircleSpeed(float scale)
    {
        var speed = _levelSpeed / scale;
        return speed;
    }

    private Sprite GetSprite(float scale)
    {
        var coef = scale / _maxCircleScale;
        var size = _prefabSettings.GetFigureTextureSize(coef);
        var sprite = _sprites[size];
        return sprite;
    }

    public void StopGame()
    {
        StopAllCoroutines();
        
        _pool.Clear();
    }

    private void MoveCircle(Circle circle, float height) 
    {
        var startPosition = circle.transform.position;
        var finishPosition = new Vector3(startPosition.x, -_screenBounds.y - height, startPosition.z);
        var routine = StartCoroutine(Move(circle, startPosition, finishPosition, circle.Speed));
        
        _routines.Add(circle.Id, routine);
    }

    private IEnumerator Move(Circle circle, Vector3 startPoint, Vector3 finishPoint, float speed)
    {
        float step = 0;
        while (step < 1)
        {
            circle.transform.position = Vector3.Lerp(startPoint, finishPoint, step);
            step += speed * Time.deltaTime;
            yield return null;
        }

        circle.transform.position = finishPoint;
        _pool.Release(circle);
    }
}