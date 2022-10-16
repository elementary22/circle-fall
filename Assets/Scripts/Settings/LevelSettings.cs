using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "circle-fall/LevelSettings", order = 0)]
public class LevelSettings : ScriptableObject
{
    [SerializeField]
    private List<LevelInfo> _levelInfo;



    public LevelInfo GetLevelInfo(int level)
    {
        return _levelInfo.Find(info => info.levelNumber == level);
    }



    [Serializable]
    public class LevelInfo
    {
        public int levelNumber;
        public int scoreGoal;
        public int pointsForClick;
        public float levelSpeed;
        public float minTimeDelay;
        public float maxTimeDelay;
    }
}