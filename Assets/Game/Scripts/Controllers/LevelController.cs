using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelController : Singleton<LevelController>
{
    public static event Action LevelLoadStarting;
    public static event Action<int> LevelLoaded;
    public static event Action LevelCompleted;

    public static event Action<string> GameCompleted;

    [SerializeField] private List<LevelStruct> _levelList = new List<LevelStruct>();

    [SerializeField] private AudioClip _levelStartSound;
    [SerializeField] private AudioClip _levelCompleteSound;

    private GameObject _currentLevel;
    private int _currentLevelIndex = -1;

    public Transform CurrentLevelTransform => _currentLevel.transform;

    private void Start()
    {
        LoadLevel(0);
    }

    private void LoadLevel(int levelIndex)
    {
        if(levelIndex != _currentLevelIndex)
        {
            //NavMeshSwitcher.Instance.SwitchTo(levelIndex);
        }

        LevelLoadStarting?.Invoke();
        
        if(_currentLevel != null)
        {
            Destroy(_currentLevel);
        }

        _currentLevelIndex = levelIndex;

        if(_currentLevelIndex != 0 && _levelStartSound != null)
        {
            AudioSource.PlayClipAtPoint(_levelStartSound, Camera.main.transform.position);
        }

        _currentLevel = Instantiate(_levelList[levelIndex].Level);

        SkyboxChanger.Instance.ChangeSkybox(_levelList[levelIndex].Skybox);

        LevelLoaded?.Invoke(levelIndex);
    }

    public void LoadNextLevel()
    {
        int nextLevelIndex = _currentLevelIndex + 1;
        if(nextLevelIndex >= _levelList.Count)
        {
            nextLevelIndex = 0;
        }

        LoadLevel(nextLevelIndex);
    }

    public void RestartLevel()
    {
        LoadLevel(_currentLevelIndex);
    }

    public void RestartGame()
    {
        LoadLevel(0);
    }

    public void CompleteLevel()
    {
        if(_currentLevelIndex == _levelList.Count - 1)
        {
            GameCompleted?.Invoke(GameEndTimer.Instance.GetTimer().ToString(@"mm\:ss"));
        }
        else
        {
            LevelCompleted?.Invoke();
        }
    }

    public int GetCurrentLevelIndex()
    {
        return _currentLevelIndex;
    }

    [Serializable]
    public class LevelStruct
    {
        public GameObject Level;
        public Material Skybox;
    }
}
