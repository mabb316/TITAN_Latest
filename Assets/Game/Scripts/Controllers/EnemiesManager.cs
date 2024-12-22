using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemiesManager : MonoBehaviour
{
    public static event Action NoEnemiesLeft;

    [SerializeField] private List<EnemyCharacter> _enemies = new List<EnemyCharacter>();
    private bool _enemiesAlerted = false;

    private void Start()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        _enemies.ForEach(enemy => enemy.Died += OnEnemyDied);
        PlayerCharacter.PlayerDied += OnPlayerDied;
        AlarmManager.Alarmed += AlertEnemies;
    }

    private void UnregisterEvents()
    {
        _enemies.ForEach(enemy => enemy.Died -= OnEnemyDied);
        PlayerCharacter.PlayerDied -= OnPlayerDied;
        AlarmManager.Alarmed -= AlertEnemies;
    }

    private void OnEnemyDied(EnemyCharacter enemy)
    {
        _enemies.Remove(enemy);

        enemy.Died -= OnEnemyDied;

        if(_enemies.Count == 0)
        {
            NoEnemiesLeft?.Invoke();
        }

        AlarmManager.Instance.InvokeAlarm();
    }

    private void OnPlayerDied()
    {
        foreach(var enemy in _enemies)
        {
            enemy.DisableAI();
        }
    }

    private void AlertEnemies()
    {
        if(_enemiesAlerted)
        {
            return;
        }
        _enemiesAlerted = true;

        _enemies.ForEach(enemy => enemy.Alert());
    }

    public int GetRemainingEnemyCount()
    {
        return _enemies.Count;
    }
}
