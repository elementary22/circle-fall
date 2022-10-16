using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PrefabSettings", menuName = "circle-fall/PrefabSettings", order = 0)]
public class PrefabSettings : ScriptableObject
{

    [SerializeField]
    private System.Collections.Generic.List<GamePrefabs> _prefabList;

    public GameObject GetFigurePrefab(Figures type)
    {
        return _prefabList.Find(prefab => prefab.type == type).prefab;
    }

    [Serializable]
    public class GamePrefabs
    {
        public Figures type;
        public GameObject prefab;
    }
}
