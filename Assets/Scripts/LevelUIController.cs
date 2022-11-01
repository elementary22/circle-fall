using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LevelSettings;

public class LevelUIController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimatorListener _animatorListener;
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

    public Action onPlay;
    public Action onClose;
    public Action onLevelCompleted;
    public Action onEndAnimationComplete;

    public void Init(LevelInfo info)
    {
        _levelInfo = info;
        _timer = new Timer();
        
        _playButton.onClick.AddListener(StartLevelAnimation);
        _closeButton.onClick.AddListener(OnClose);
        _timer.onChangeTimer += UpdateTimer;
        _animatorListener.onComplete += EndLevelAnimation;

        GetLevelBackground();
        CheckLevel();
    }

    private void GetLevelBackground()
    {
        BundleLoader.Instance.Download(_levelInfo.levelNumber, SetLevelBackground);
    }

    private void SetLevelBackground(Sprite bg)
    {
        _background.sprite = bg;
    }

    private void CheckLevel()
    {
        if (_levelInfo.levelNumber > 1)
            StartLevelAnimation();
    }

    private void StartLevelAnimation()
    {
        _levelNumberText.text = $"{Localization.Instance.GetKey("tf_level")} {_levelInfo.levelNumber}";
        _animator.SetTrigger(Config.StartAnimationTrigger);
    }

    private void EndLevelAnimation()
    {
        _animator.SetTrigger(Config.EndAnimationTrigger);
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
        _timerRoutine = StartCoroutine(_timer.StartTimerCo());
    }
    private void OnClose()
    {
        StopGame();
        onClose?.Invoke();
    }

    public void StopGame()
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
        StopCoroutine(_timerRoutine);
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
        _levelNumberText.text = Localization.Instance.GetKey("tf_endgame");
        _animator.SetTrigger(Config.StartAnimationTrigger);
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveListener(StartLevelAnimation);
        _closeButton.onClick.RemoveListener(OnClose);
        _timer.onChangeTimer -= UpdateTimer;
        _animatorListener.onComplete -= EndLevelAnimation;
    }
}
