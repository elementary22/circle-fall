using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TextureGenerator
{
    private Texture2D[] _textureArray;
    private Texture2D _atlas;
    private Rect[] _textures;
    private int _atlasLength;
    private Action<Dictionary<TextureSize, Sprite>> onGenerate;

    public void DeleteTextures()
    {
        UnityEngine.Object.Destroy(_atlas);
        for (int i = 0; i < _atlasLength; i++)
        {
            UnityEngine.Object.Destroy(_textureArray[i]);
        }
        _textureArray = null;
        _atlas = null;
    }

    public void StartGeneration(Action<Dictionary<TextureSize, Sprite>> callback)
    {
        onGenerate = callback;
        Create();
    }

    private void Create()
    {
        _atlasLength = Enum.GetNames(typeof(TextureSize)).Length;
        _atlas = new Texture2D(512, 512);
        _atlas.hideFlags = HideFlags.HideAndDontSave;
        _atlas.filterMode = FilterMode.Bilinear;
        _textures = CreateTextureAtlas();
        GetSprites();
    }

    private void GetSprites()
    {
        Dictionary<TextureSize, Sprite> sprites = new Dictionary<TextureSize, Sprite>();
        for (int i = 0; i < _atlasLength; i++)
        {
            Rect rect = _textures[i];
            float rectOffset = _atlas.width * rect.xMin;
            int size = (int)(_atlas.width * rect.xMax - rectOffset);
            Sprite sprite = Sprite.Create(_atlas, new Rect(rectOffset, 0, size, size), new Vector2(0.5f, 0.5f), size, 0, SpriteMeshType.FullRect);

            sprites.Add((TextureSize)size, sprite);
        }
        onGenerate?.Invoke(sprites);
    }

    private Rect[] CreateTextureAtlas()
    {
        _textureArray = new Texture2D[_atlasLength];
        Rect[] sizeList = new Rect[_atlasLength];
        int size = 32;
        for (int i = 0; i < _atlasLength; i++)
        {
            _textureArray[i] = DrawCircle(new Texture2D(size, size, TextureFormat.RGBA64, 0, false), UnityEngine.Random.ColorHSV(), size / 2, size / 2, size / 2);
            size *= 2;
        }
        Rect[] textures = _atlas.PackTextures(_textureArray, 2, _atlas.width);
        return textures;
    }

    private Texture2D DrawCircle(Texture2D texture, Color color, int x, int y, int radius)
    {
        float rSquared = radius * radius;

        for (int u = x - radius; u < x + radius + 1; u++)
        {
            for (int v = y - radius; v < y + radius + 1; v++)
            {
                if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                {
                    color.a = 1;
                    texture.SetPixel(u, v, color);
                }
                else
                {
                    color.a = 0;
                    texture.SetPixel(u, v, color);
                }
            }
        }
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        return texture;
    }
}
