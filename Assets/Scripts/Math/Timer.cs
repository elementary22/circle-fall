using UnityEngine;
using System;
using Zenject;

public class Timer : ITickable
{
    private float _timer;
    private bool _isTicking;

    public Action<string> OnChangeTimer;

    public void StartTimer()
    {
        if(_isTicking) return;
        _isTicking = true;
    }

    public void StopTimer()
    {
        _isTicking = false;
        _timer = 0f;
    }

    private void UpdateTimerDisplay(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        OnChangeTimer?.Invoke($"{minutes:00}:{seconds:00}");
    }

    public void Tick()
    {
        if (!_isTicking) return;
        UpdateTimerDisplay(_timer);
        _timer += Time.deltaTime;
    }
}