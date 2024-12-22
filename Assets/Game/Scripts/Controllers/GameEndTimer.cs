using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndTimer : Singleton<GameEndTimer>
{
    private float _currentTime = 0f;

    private bool _isTimerTicking = false;

    private void Start()
    {
        RegisterEvents();
    }

    private void OnDestroy()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        LevelController.LevelLoaded += OnLevelLoaded;
        LevelController.LevelCompleted += PauseTimer;
    }

    private void UnregisterEvents()
    {     
        LevelController.LevelLoaded -= OnLevelLoaded;
        LevelController.LevelCompleted -= PauseTimer;
    }

    private void Update()
    {
        if(!_isTimerTicking)
        {
            return;
        }
        _currentTime += Time.deltaTime;
    }

    private void OnLevelLoaded(int levelIndex)
    {
        if(levelIndex == 0)
        {
            RestartTimer();
        }
        else
        {
            ContinueTimer();
        }
    }

    public void RestartTimer()
    {
        _currentTime = 0f;

        _isTimerTicking = true;
    }

    public void ContinueTimer()
    {
        _isTimerTicking = true;
    }

    public void PauseTimer()
    {
        _isTimerTicking = false;
    }

    public TimeSpan GetTimer()
    {
        TimeSpan time = TimeSpan.FromSeconds(_currentTime);
        return time;
    }
}
