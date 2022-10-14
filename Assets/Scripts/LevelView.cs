using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelView : MonoBehaviour
{
    [SerializeField]
    private Transform _circleContainer;
    [SerializeField]
    private float _minCircleScale;
    [SerializeField]
    private float _standartSpeed;
    private GameObject _circlePrefab;
    private float _maxCircleScale;
    private Vector2 _screenBounds;
    private Dictionary<int, Coroutine> _routines;
    private int _id = 0;

    public void Init(PrefabSettings prefabSettings)
    {
        _circlePrefab = prefabSettings.GetCirclePrefab();
        _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        _maxCircleScale = GetMaxScale();

        _routines = new Dictionary<int, Coroutine>();
    }

    public void SpawnCircle()
    {
        float scale = Random.Range(_minCircleScale, _maxCircleScale);

        GameObject cicrlePrefab = GameObject.Instantiate(_circlePrefab);
        Circle circle = cicrlePrefab.GetComponent<Circle>();
        cicrlePrefab.transform.SetParent(_circleContainer, false);

        circle.onMove += MoveCircle;
        circle.onClicked += OnCircleClicked;

        circle.id = _id;
        circle.SetSize(scale);
        circle.SetSpriteColor(GetRandomColor());
        circle.SetSpeed(GetCircleSpeed(scale));
        circle.SetTransofrmPosition(_screenBounds);

        _id++;
    }

    private void OnCircleClicked(Circle circle)
    {   
        if (_routines[circle.id] == null)
            return;
        StopCoroutine(_routines[circle.id]);
        circle.Dispose();
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
        float speed = _standartSpeed / scale;
        return speed;
    }

    private Color GetRandomColor()
    {
        Color randomColor = Random.ColorHSV();
        return randomColor;
    }

    private void MoveCircle(Circle circle, float height)
    {
        Vector3 startPosition = circle.transform.position;
        Vector3 finishPosition = new Vector3(startPosition.x, -_screenBounds.y - height, startPosition.z);

        Coroutine routine = StartCoroutine(Move(circle, startPosition, finishPosition, circle.speed));
        _routines.Add(circle.id, routine);
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
        GameObject.Destroy(circle.gameObject);
    }
}
