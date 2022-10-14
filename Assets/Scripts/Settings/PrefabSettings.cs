using UnityEngine;

[CreateAssetMenu(fileName = "PrefabSettings", menuName = "circle-fall/PrefabSettings", order = 0)]
public class PrefabSettings : ScriptableObject {
    
    [SerializeField]
    private GameObject _circlePrefab;

    public GameObject GetCirclePrefab() {
        return _circlePrefab;
    }

}
