using System;
using UnityEngine;

public class Circle : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    private float _speed;
    public float speed { get { return _speed; } }
    private int _id;
    public int id { get { return _id; } set { _id = value; } }
    private float _scale;
    public float scale { get { return _scale; } }

    public Action<Circle, float> onMove;
    public Action<Circle> onClicked;

    private void OnMouseDown()
    {
        onClicked?.Invoke(this);
    }

    public void SetSize(float scale)
    {
        _scale = scale;
        var newScale = new Vector3(scale, scale, 0.2f);
        transform.localScale = newScale;
    }

    public void SetSprite(Sprite sprite) => _spriteRenderer.sprite = sprite;

    public void SetSpeed(float speed) => _speed = speed;

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
        this.gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {
        this.gameObject.SetActive(false);
    }
}
