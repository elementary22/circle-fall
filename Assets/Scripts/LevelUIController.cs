using System;
using System.Threading;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LevelSettings;

public class LevelUIController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _background;
    [SerializeField] private TextMeshProUGUI _levelNumberText;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _closeButton;
    
    private LevelInfo _levelInfo;
    private Timer _timer;
    private int _score;
    private Coroutine _timerRoutine;
    private bool _isCompleted;
    private Tween _scoreTween;
    private CancellationTokenSource _cts;

    public Action onPlay;
    public Action onClose;
    public Action onLevelCompleted;
    public Action onEndAnimationComplete;

    public void Init(LevelInfo info)
    {
        _levelInfo = info;
        _timer = new Timer();
        
        _playButton.onClick.AddListener(StartLevel);
        _closeButton.onClick.AddListener(OnClose);
        _timer.onChangeTimer += UpdateTimer;

        GetLevelBackground();
        CheckLevel();
    }

    private async void GetLevelBackground()
    {
        _cts = new CancellationTokenSource();
        await BundleLoader.Instance.Download(_levelInfo.levelNumber, _cts, SetLevelBackground);
    }

    private void SetLevelBackground(Sprite bg)
    {
        _background.sprite = bg;
    }

    private void CheckLevel()
    {
        if (_levelInfo.levelNumber > 1)
            StartLevel();
    }
    private void StartLevel()
    {
        _levelNumberText.text = $"{Localization.Instance.GetKey($"{Config.LocLevelKey}")} {_levelInfo.levelNumber}";
        LevelTextAnimation();
    }

    private void LevelTextAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_levelNumberText.DOFade(1f, 0.75f)).SetEase(Ease.Linear);
        sequence.Append(_levelNumberText.DOFade(0f, 0.75f)).SetEase(Ease.Linear);
        sequence.AppendCallback(EndLevel);
    }
    
    private void EndLevel()
    {
        if (_isCompleted)
        {
            onEndAnimationComplete?.Invoke();
            return;
        }
        OnPlay();
    }

    private void OnPlay()
    {
        _scoreText.gameObject.SetActive(true);
        _timerText.gameObject.SetActive(true);
        _playButton.interactable = false;
        SetTimer();
        
        onPlay?.Invoke();
    }
    private void SetTimer()
    {
        _timer.StartTimer();
    }
    private void OnClose()
    {
        StopGame();
        onClose?.Invoke();
    }

    private void StopGame()
    {
        _scoreText.gameObject.SetActive(false);
        _timerText.gameObject.SetActive(false);
        _playButton.interactable = true;
        StopTimer();
        ResetScore();
    }

    private void ResetScore()
    {
        _score = 0;
        _scoreText.text = _score.ToString();
    }

    private void StopTimer()
    {
        _timer.StopTimer();
    }

    private void UpdateTimer(string time)
    {
        _timerText.text = time;
    }

    public void UpdateScore(float circleScale, float maxScale)
    {
        var result = (int)(maxScale / circleScale * _levelInfo.pointsForClick);

        _score += result;
        _scoreText.text = _score.ToString();
        _scoreTween.Kill();
        _scoreText.transform.localScale = Vector3.one;
        _scoreTween = _scoreText.transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0), 0.5f);

        CheckLevelCompleted();
    }

    private void CheckLevelCompleted()
    {
        if (_score >= _levelInfo.scoreGoal)
            onLevelCompleted?.Invoke();
    }

    public void CompleteGame()
    {
        StopGame();
        _isCompleted = true;
        _levelNumberText.text = Localization.Instance.GetKey($"{Config.LocEndLevelKey}");
        LevelTextAnimation();
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveListener(StartLevel);
        _closeButton.onClick.RemoveListener(OnClose);
        _timer.onChangeTimer -= UpdateTimer;
    }
}
