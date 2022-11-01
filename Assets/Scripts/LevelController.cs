using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LevelSettings;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    private PrefabSettings _prefabSettings;
    [SerializeField]
    private LevelSettings _levelSettings;
    [SerializeField]
    private LevelView _levelView;
    [SerializeField]
    private LevelUIController _levelUIController;
    private LevelInfo _levelInfo;
    private int _gameLevel;
    private int _maxLevel = 3;

    public Action onGameComplete;

    private void Start()
    {
        CheckLevelData();
        
        _levelInfo = _levelSettings.GetLevelInfo(_gameLevel);
        _levelUIController.Init(_levelInfo);
        _levelView.Init(_prefabSettings, _levelInfo.levelSpeed);
        
        _levelUIController.onPlay += PlayGame;
        _levelUIController.onClose += StopGame;
        _levelUIController.onLevelCompleted += CompleteLevel;
        _levelUIController.onEndAnimationComplete += SaveAndLoadLevel;
        _levelView.onFigureClick += ScorePoints;

        onGameComplete += _levelUIController.CompleteGame;
    }

    private void CheckLevelData()
    {
        if (PlayerPrefs.HasKey("level"))
            _gameLevel = PlayerPrefs.GetInt("level");
        else
            _gameLevel = 1;
    }

    private void PlayGame()
    {
        StartCoroutine(SpawnCircles());
    }

    private void StopGame()
    {
        _levelView.StopGame();
        StopAllCoroutines();
    }

    private void CompleteLevel()
    {
        if (_gameLevel == _maxLevel)
        {
            StopGame();
            CompleteGame();
            return;
        }
        _gameLevel++;
        SaveAndLoadLevel();
    }

    private void CompleteGame()
    {
        ResetLevel();
        onGameComplete?.Invoke();
    }

    private void ResetLevel()
    {
        _gameLevel = 1;
    }

    private void SaveAndLoadLevel()
    {
        PlayerPrefs.SetInt("level", _gameLevel);
        SceneManager.LoadScene("game");
    }

    private void ScorePoints(float circleScale, float maxScale)
    {
        _levelUIController.UpdateScore(circleScale, maxScale);
    }

    private IEnumerator SpawnCircles()
    {
        _levelView.SpawnFigure();
        yield return null;
        StartCoroutine(SetSpawnDelay());
    }

    private IEnumerator SetSpawnDelay()
    {
        float randomDelayTime = UnityEngine.Random.Range(_levelInfo.minTimeDelay, _levelInfo.maxTimeDelay);
        yield return new WaitForSeconds(randomDelayTime);
        StartCoroutine(SpawnCircles());
    }

    private void OnDestroy()
    {
        _levelUIController.onPlay -= PlayGame;
        _levelUIController.onClose -= StopGame;
        _levelUIController.onLevelCompleted -= CompleteLevel;
        _levelView.onFigureClick -= ScorePoints;
        onGameComplete -= _levelUIController.CompleteGame;
        _levelUIController.onEndAnimationComplete -= SaveAndLoadLevel;
    }
}
