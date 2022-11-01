using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static LevelSettings;

public class LevelController : MonoBehaviour
{
    [SerializeField] private PrefabSettings _prefabSettings;
    [SerializeField] private LevelSettings _levelSettings;
    [SerializeField] private FigureSpawner _figureSpawner;
    [SerializeField] private LevelUIController _levelUIController;
    
    private LevelInfo _levelInfo;
    private int _gameLevel;
    private const int MaxLevel = 3;

    public Action onGameComplete;

    private void Start()
    {
        CheckLevelData();
        
        _levelInfo = _levelSettings.GetLevelInfo(_gameLevel);

        _levelUIController.Init(_levelInfo);
        _levelUIController.onPlay += PlayGame;
        _levelUIController.onClose += StopGame;
        _levelUIController.onLevelCompleted += CompleteLevel;
        _levelUIController.onEndAnimationComplete += SaveAndLoadLevel;
        
        _figureSpawner.Init(_prefabSettings, _levelInfo);
        _figureSpawner.onFigureClick += ScorePoints;

        onGameComplete += _levelUIController.CompleteGame;
    }

    private void CheckLevelData()
    {
        _gameLevel = PlayerPrefs.GetInt("level", 1);
    }

    private void PlayGame()
    {
        _figureSpawner.StartSpawn();
    }

    private void StopGame()
    {
        _figureSpawner.StopSpawn();
    }

    private void CompleteLevel()
    {
        if (_gameLevel == MaxLevel)
        {
            CompleteGame();
            return;
        }
        StopGame();
        _gameLevel++;
        SaveAndLoadLevel();
    }

    private void CompleteGame()
    {
        StopGame();
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

    private void OnDestroy()
    {
        _levelUIController.onPlay -= PlayGame;
        _levelUIController.onClose -= StopGame;
        _levelUIController.onLevelCompleted -= CompleteLevel;
        _figureSpawner.onFigureClick -= ScorePoints;
        onGameComplete -= _levelUIController.CompleteGame;
        _levelUIController.onEndAnimationComplete -= SaveAndLoadLevel;
    }
}
