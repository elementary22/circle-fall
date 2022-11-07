using System.Threading;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static LevelSettings;

public class LevelUIView : MonoBehaviour, IView
{
    [Inject] private readonly SignalBus _signalBus;
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
    private Tween _scoreTween;
    private CancellationTokenSource _cts;
    private bool _isCompleted;

    public void Init(LevelInfo info)
    {
        _levelInfo = info;
        _timer = new Timer();
        
        _playButton.onClick.AddListener(StartLevel);
        _closeButton.onClick.AddListener(OnClose);
        _timer.onChangeTimer += UpdateTimer;

        GetLevelBackground();
    }

    private void Start()
    {
        _signalBus.Fire(new OnViewEnableViewSignal(this));
    }
    // CHECK THIS 
    private void OnApplicationQuit()
    {
        _signalBus.TryFire(new OnViewDisableViewSignal(this));
    }

    public async void GetLevelBackground()
    {
        _cts = new CancellationTokenSource();
        await BundleLoader.Instance.Download(_levelInfo.levelNumber, _cts, SetLevelBackground);
    }

    private void SetLevelBackground(Sprite bg)
    {
        _background.sprite = bg;
    }

    private void StartLevel()
    {
        StartLevelAnimation();
    }

    public void StartLevelAnimation()
    {
        var text = $"{Localization.Instance.GetKey($"{Config.LocLevelKey}")} {_levelInfo.levelNumber}";;
        LevelAnimation(text);
    }

    private void LevelAnimation(string text)
    {
        _levelNumberText.text = text;
        var sequence = DOTween.Sequence();
        sequence.Append(_levelNumberText.DOFade(1f, Config.ScaleDuration)).SetEase(Ease.Linear);
        sequence.Append(_levelNumberText.DOFade(0f, Config.ScaleDuration)).SetEase(Ease.Linear);
        sequence.AppendCallback(OnAnimationComplete);
    }

    private void OnAnimationComplete()
    {
        if (!_isCompleted)
        {
            OnPlay();
        } else _signalBus.Fire<OnResetGameViewSignal>();
    }
    
    private void OnPlay()
    {
        _scoreText.gameObject.SetActive(true);
        _timerText.gameObject.SetActive(true);
        _playButton.interactable = false;
        SetTimer();
        
        _signalBus.Fire<OnPlayViewSignal>();
    }
    
    private void SetTimer()
    {
        _timer.StartTimer();
    }
    
    private void OnClose()
    {
        StopGame();
        _signalBus.Fire<OnCloseViewSignal>();
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

    public void UpdateLevelInfo(LevelInfo levelInfo)
    {
        _levelInfo = levelInfo;
    }
    
    public void UpdateScore(float circleScale, float maxScale)
    {
        var result = (int)(maxScale / circleScale * _levelInfo.pointsForClick);

        _score += result;
        _scoreText.text = _score.ToString();
        _scoreTween.Kill();
        _scoreText.transform.localScale = Vector3.one;
        _scoreTween = _scoreText.transform.DOPunchScale(new Vector3(Config.ScoreScaleSize, Config.ScoreScaleSize, 0), Config.ScoreScaleDuration);

        CheckLevelCompleted();
    }

    private void CheckLevelCompleted()
    {
        if (_score >= _levelInfo.scoreGoal)
            _signalBus.Fire<OnLevelCompleteViewSignal>();
    }

    public void CompleteGame()
    {
        StopGame();
        _isCompleted = true;
        var text = Localization.Instance.GetKey($"{Config.LocEndLevelKey}");
        LevelAnimation(text);
    }

    private void OnDestroy()
    {
        _playButton.onClick.RemoveListener(StartLevel);
        _closeButton.onClick.RemoveListener(OnClose);
        _timer.onChangeTimer -= UpdateTimer;
    }
}
