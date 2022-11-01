using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PrefabSettings", menuName = "circle-fall/PrefabSettings", order = 0)]
public class PrefabSettings : ScriptableObject
{
    [SerializeField]
    private List<Figure> _figuresPrefabs;
    [SerializeField]
    private List<FigureTextureSize> _figureSize;
    
    public Figure GetFigurePrefab(Figure figure)
    {
        return _figuresPrefabs.Find(item => item.GetFigureType() == figure);
    }

    public List<Figure> GetFigures()
    {
        return _figuresPrefabs;
    }

    public TextureSize GetFigureTextureSize(float coef)
    {
        var size = TextureSize.SMALL;
        foreach (var textureSize in _figureSize.Where(textureSize => coef <= textureSize.scale))
        {
            size = textureSize.size;
            break;
        }
        return size;
    }

    [Serializable]
    public class FigureTextureSize
    {
        public TextureSize size;
        public float scale;
    }
}
