using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStatusUI : MonoBehaviour
{
    [SerializeField] private GameObject _backgroundFade;
    [SerializeField] private GameObject _levelFailedUI;
    [SerializeField] private GameObject _levelCompleteUI;
    [SerializeField] private GameObject _gameCompleteUI;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _restartGameButton;

    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _gameFinishTime;

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
        PlayerCharacter.PlayerDied += OpenLevelFailedUI;
        LevelController.LevelLoaded += OnLevelLoaded;
        LevelController.LevelCompleted += OpenLevelCompleteUI;
        LevelController.GameCompleted += OpenGameCompleteUI;

        _nextLevelButton.onClick.AddListener(LevelController.Instance.LoadNextLevel);
        _restartButton.onClick.AddListener(LevelController.Instance.RestartLevel);
        _restartGameButton.onClick.AddListener(LevelController.Instance.RestartGame);
    }

    private void UnregisterEvents()
    {
        PlayerCharacter.PlayerDied -= OpenLevelFailedUI;
        LevelController.LevelLoaded -= OnLevelLoaded;
        LevelController.LevelCompleted -= OpenLevelCompleteUI;
        LevelController.GameCompleted -= OpenGameCompleteUI;

        _nextLevelButton.onClick.RemoveListener(LevelController.Instance.LoadNextLevel);
        _restartButton.onClick.RemoveListener(LevelController.Instance.RestartLevel);
        _restartGameButton.onClick.RemoveListener(LevelController.Instance.RestartGame);
    }

    private void OnLevelLoaded(int levelIndex)
    {
        CloseAllUI();
        _levelText.text = "Level " + (levelIndex + 1);
    }

    private void OpenLevelFailedUI()
    {
        SetBackgroundFadeActive(true);
        
        StartCoroutine(WaitAndOpenLevelFailedUI());
    }

    private IEnumerator WaitAndOpenLevelFailedUI()
    {
        yield return new WaitForSeconds(1f);

        _levelFailedUI.SetActive(true);
    }

    private void OpenLevelCompleteUI()
    {
        SetBackgroundFadeActive(true);

        StartCoroutine(WaitAndOpenLevelCompleteUI());
    }

    private IEnumerator WaitAndOpenLevelCompleteUI()
    {
        yield return new WaitForSeconds(1f);

        _levelCompleteUI.SetActive(true);
    }

    private void OpenGameCompleteUI(string time)
    {
        SetBackgroundFadeActive(true);

        StartCoroutine(WaitAndOpenGameCompleteUI(time));
    }

    private IEnumerator WaitAndOpenGameCompleteUI(string time)
    {
        yield return new WaitForSeconds(1f);

        _gameCompleteUI.SetActive(true);
        _gameFinishTime.text = time;
    }

    private void CloseAllUI()
    {
        _levelFailedUI.SetActive(false);
        _levelCompleteUI.SetActive(false);
        _gameCompleteUI.SetActive(false);

        SetBackgroundFadeActive(false);
    }

    private void SetBackgroundFadeActive(bool isActive)
    {
        _backgroundFade.SetActive(isActive);
    }
}
