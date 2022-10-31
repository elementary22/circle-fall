using System;
using UnityEngine;

public class Circle : MonoBehaviour, iPoolable
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    public float Speed { get; private set; }
    public int Id { get; set; }
    public float Scale { get; private set; }

    public Action<Circle, float> onMove;
    public Action<Circle> onClicked;

    public Circle(float scale, int id, float speed)
    {
        this.Scale = scale;
        this.Id = id;
        this.Speed = speed;
    }

    private void OnMouseDown()
    {
        onClicked?.Invoke(this);
    }

    public void SetSize(float scale)
    {
        Scale = scale;
        var newScale = new Vector3(scale, scale, 0.2f);
        transform.localScale = newScale;
    }

    public void SetSprite(Sprite sprite) => _spriteRenderer.sprite = sprite;

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
        onMove?.Invoke(this, height);
    }

    public void Dispose()
    {
        onMove = null;
        onClicked = null;
        Destroy(this.gameObject);
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
        Dispose();
    }
}
