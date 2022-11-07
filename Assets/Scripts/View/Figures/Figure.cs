using System;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class Figure : MonoBehaviour, iPoolable
{
    [SerializeField] protected SpriteRenderer _spriteRenderer;
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
        onMove?.Invoke(this, _spriteRenderer.bounds.size.y * 0.5f);
    }

    protected void OnMouseDown()
    {
        onClicked?.Invoke(this);
        onClicked = null;
    }

    private void SetSize(float scale)
    {
        Scale = scale;
        var newScale = new Vector3(scale, scale, 0.2f);
        transform.localScale = newScale;
    }

    private void SetColor()
    {
        _spriteRenderer.color = Random.ColorHSV();
    }

    private void SetSpeed(float speed)
    {
        Speed = speed;
    }

    private void SetTransformPosition(Vector2 screenBounds)
    {
        var bounds = _spriteRenderer.bounds;
        var width = bounds.size.x * 0.5f;
        var height = bounds.size.y * 0.5f;
        var pos = transform.position;

        pos.x = Random.Range(-screenBounds.x + width, screenBounds.x - width);
        pos.y = screenBounds.y + height;

        transform.position = pos;
    }

    public void GetFromPool()
    {
        gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {
        Dispose();
        gameObject.SetActive(false);
    }

    public void DestroyPoolObject()
    {
        Dispose();
        Destroy(gameObject);
    }

    private void Dispose()
    {
        onMove = null;
        onClicked = null;
    }

    public class Factory : PlaceholderFactory<UnityEngine.Object, Figure>
    {
        
    }
}