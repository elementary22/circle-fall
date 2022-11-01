using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Figure : MonoBehaviour, iPoolable
{
    [SerializeField]
    protected SpriteRenderer _spriteRenderer;
    public float Speed { get; set; }
    public int Id { get; set; }
    public float Scale { get; set; }

    public Action<Figure, float> onMove;
    public Action<Figure> onClicked;

    public void Init(float scale, float speed, Vector2 screenBounds)
    {
        SetSize(scale);
        SetColor();
        SetSpeed(speed);
        SetTransformPosition(screenBounds);
        onMove?.Invoke(this, _spriteRenderer.bounds.size.y / 2);
    }
    
    public Figure GetFigureType()
    {
        return this;
    }
    
    protected void OnMouseDown()
    {
        onClicked?.Invoke(this);
    }

    public void SetSize(float scale)
    {
        Scale = scale;
        var newScale = new Vector3(scale, scale, 0.2f);
        transform.localScale = newScale;
    }

    public void SetColor()
    {
        _spriteRenderer.color = Random.ColorHSV();
    }

    public void SetSpeed(float speed) => Speed = speed;

    public void SetTransformPosition(Vector2 screenBounds)
    {
        var bounds = _spriteRenderer.bounds;
        var width = bounds.size.x / 2;
        var height = bounds.size.y / 2;

        var pos = this.transform.position;
        pos.x = UnityEngine.Random.Range(-screenBounds.x + width, screenBounds.x - width);
        pos.y = screenBounds.y + height;

        transform.position = pos;
    }

    public void Dispose()
    {
        onMove = null;
        onClicked = null;
    }

    public void GetFromPool()
    {
        gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    public void DestroyPoolObject()
    {
        onMove = null;
        onClicked = null;
        Destroy(this.gameObject);
    }
}
