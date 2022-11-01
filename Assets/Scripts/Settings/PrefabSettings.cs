using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PrefabSettings", menuName = "circle-fall/PrefabSettings", order = 0)]
public class PrefabSettings : ScriptableObject
{
    [field:SerializeField]
    public List<Figure> FiguresPrefabs { get; private set; }
}
