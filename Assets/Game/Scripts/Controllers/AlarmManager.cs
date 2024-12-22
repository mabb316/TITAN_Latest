using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AlarmManager : Singleton<AlarmManager>
{
    public static event Action Alarmed;

    private void Start()
    {
        PlayerCharacter.PlayerInitialized += OnPlayerInitialized;
    }

    private void OnDisable()
    {
        PlayerCharacter.PlayerInitialized -= OnPlayerInitialized;
    }

    public void InvokeAlarm()
    {
        Alarmed?.Invoke();
    }

    private void OnPlayerInitialized(Transform player)
    {
        if(LevelController.Instance.GetCurrentLevelIndex() == 0)
        {
            return;
        }

        InvokeAlarm();
    }
    
}
