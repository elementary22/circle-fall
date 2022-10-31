using UnityEngine;
using System.Collections;
using System;

public class Timer
{
    private float _timer;
    private string _firstMinute;
    private string _secondMinute;
    private string _firstSecond;
    private string _secondSecond;
    public bool isTicking = false;

    public Action<string> onChangeTimer;

    public void StartTimer()
    {
        isTicking = true;
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

        string currentTime = string.Format("{00:00}{1:00}", minutes, seconds);

        _firstMinute = currentTime[0].ToString();
        _secondMinute = currentTime[1].ToString();
        _firstSecond = currentTime[2].ToString();
        _secondSecond = currentTime[3].ToString();
        onChangeTimer?.Invoke($"{_firstMinute}{_secondMinute} : {_firstSecond}{_secondSecond}");
    }

    public IEnumerator StartTimerCo()
    {
        while (isTicking)
        {
            _timer += Time.deltaTime;
            UpdateTimerDisplay(_timer);
            yield return null;
        }
    }
}
