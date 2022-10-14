using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private PrefabSettings _prefabSettings;
    [SerializeField]
    private LevelView _levelView;
    [SerializeField]
    private float minDelayTime;
    [SerializeField]
    private float maxDelayTime;

    private void Start()
    {
        _levelView.Init(_prefabSettings);
        PlayGame();
    }

    private void PlayGame() => StartCoroutine(SpawnCircles());

    private IEnumerator SpawnCircles()
    {
        _levelView.SpawnCircle();
        yield return null;
        StartCoroutine(SetSpawnDelay());
    }

    private IEnumerator SetSpawnDelay()
    {
        float randomDelayTime = UnityEngine.Random.Range(minDelayTime, maxDelayTime);
        yield return new WaitForSeconds(randomDelayTime);
        StartCoroutine(SpawnCircles());
    }
}
