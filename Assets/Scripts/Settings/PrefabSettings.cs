using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PrefabSettings", menuName = "circle-fall/PrefabSettings", order = 0)]
public class PrefabSettings : ScriptableObject
{
    [SerializeField]
    private List<GamePrefabs> _prefabList;
    [SerializeField]
    private List<FigureTextureSize> _figureSize;
    
    public T GetFigurePrefab<T>(Figures type)
    {
        return _prefabList.Find(prefab => prefab.type == type).prefab.GetComponent<T>();
    }

    public TextureSize GetFigureTextureSize(float coef)
    {
        var size = TextureSize.SMALL;
        foreach (var textureSize in _figureSize)
        {
            if (!(coef <= textureSize.scale)) continue;
            size = textureSize.size;
            break;
        }
        return size;
    }

    [Serializable]
    public class GamePrefabs
    {
        public Figures type;
        public GameObject prefab;
    }

    [Serializable]
    public class FigureTextureSize
    {
        public TextureSize size;
        public float scale;
    }
}
