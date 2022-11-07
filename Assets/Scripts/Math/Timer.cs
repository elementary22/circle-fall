using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using Zenject;

public class Timer : ITickable
{
    private float _timer;
    private string _firstMinute;
    private string _secondMinute;
    private string _firstSecond;
    private string _secondSecond;
    private bool isTicking;

    public Action<string> onChangeTimer;

    public void StartTimer()
    {
        isTicking = true;
        StartTimerAsync().Forget();
    }

    public void StopTimer()
    {
        isTicking = false;
        _timer = 0f;
    }

    private void UpdateTimerDisplay(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        var currentTime = $"{minutes:00}{seconds:00}";

        _firstMinute = currentTime[0].ToString();
        _secondMinute = currentTime[1].ToString();
        _firstSecond = currentTime[2].ToString();
        _secondSecond = currentTime[3].ToString();
        onChangeTimer?.Invoke($"{_firstMinute}{_secondMinute} : {_firstSecond}{_secondSecond}");
    }
    
    private async UniTaskVoid StartTimerAsync()
    {
        while (isTicking)
        {
            UpdateTimerDisplay(_timer);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            _timer += 1;
        }
    }

    public void Tick()
    {
        throw new NotImplementedException();
    }
}