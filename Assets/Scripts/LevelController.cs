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
    private int _gameLevel;
    private LevelInfo _levelInfo;
    private TextureGenerator _textureGenerator;
    private int _maxLevel = 3;

    public Action onGameComplete;

    private void Start()
    {
        CheckLevelData();

        _textureGenerator = new TextureGenerator();
        _levelInfo = _levelSettings.GetLevelInfo(_gameLevel);
        _levelUIController.Init(_levelInfo);

        _levelUIController.onPlay += PlayGame;
        _levelUIController.onClose += StopGame;
        _levelUIController.onLevelCompleted += CompleteLevel;
        _levelUIController.onEndAnimationComplete += SaveAndLoadLevel;
        _levelView.onCircleClick += ScorePoints;

        onGameComplete += _levelUIController.CompleteGame;

        _textureGenerator.StartGeneration(OnGenerateSprite);
    }

    private void OnGenerateSprite(Dictionary<TextureSize, Sprite> dictionary)
    {
        _levelView.Init(_prefabSettings, dictionary, _levelInfo.levelSpeed);
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
        _textureGenerator.DeleteTextures();
        SaveAndLoadLevel();
    }

    private void CompleteGame()
    {
        ResetLevel();
        _textureGenerator.DeleteTextures();
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
        _levelView.SpawnCircle();
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
        _levelView.onCircleClick -= ScorePoints;
        onGameComplete -= _levelUIController.CompleteGame;
        _levelUIController.onEndAnimationComplete -= SaveAndLoadLevel;
    }
}
