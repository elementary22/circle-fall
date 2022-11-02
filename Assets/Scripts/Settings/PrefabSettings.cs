using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PrefabSettings", menuName = "circle-fall/PrefabSettings", order = 0)]
public class PrefabSettings : ScriptableObject
{

    [SerializeField]
    private List<GamePrefabs> _prefabList;
    [SerializeField]
    private List<FigureTexureSize> _figureSize;

    public GameObject GetFigurePrefab(Figures type)
    {
        return _prefabList.Find(prefab => prefab.type == type).prefab;
    }

    public TextureSize GetFigureTextureSize(float coef)
    {
        TextureSize size = TextureSize.SMALL;
        for (int i = 0; i < _figureSize.Count; i++)
        {
            if(coef <= _figureSize[i].scale) {
                size = _figureSize[i].size;
                break;
            }
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
    public class FigureTexureSize
    {
        public TextureSize size;
        public float scale;
    }
}
