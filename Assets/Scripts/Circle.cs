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
    private bool isClicked = false;
    public Action<Circle, float> onMove;
    public Action<Circle> onClicked;

    private void Update()
    {
        HandleClick();
    }

    private void OnMouseDown()
    {
        isClicked = true;
    }

    private void HandleClick()
    {
        if (isClicked)
        {
            if (Input.GetMouseButtonUp(0))
            {
                onClicked?.Invoke(this);
                isClicked = false;
            }
        }

    }

    public void SetSize(float scale)
    {
        Vector3 newScale = new Vector3(scale, scale, 0.2f);
        this.transform.localScale = newScale;
    }

    public void SetSprite(Sprite sprite) => _spriteRenderer.sprite = sprite;

    public void SetSpeed(float speed) => _speed = speed;

    public void SetSpriteColor(Color color) => _spriteRenderer.color = color;

    public void SetTransofrmPosition(Vector2 screenBounds)
    {
        float width = _spriteRenderer.bounds.size.x / 2;
        float height = _spriteRenderer.bounds.size.y / 2;

        Vector3 pos = this.transform.position;
        pos.x = UnityEngine.Random.Range(-screenBounds.x + width, screenBounds.x - width);
        pos.y = screenBounds.y + height;

        this.transform.position = pos;
        onMove?.Invoke(this, height);
    }

    public void Dispose()
    {
        onMove = null;
        onClicked = null;
        Destroy(this.gameObject);
    }
}
