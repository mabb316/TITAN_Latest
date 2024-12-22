using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndZone : MonoBehaviour
{
    [SerializeField] private Transform _elevatorDoor;
    private bool _isEnding = false;
    private bool _enemiesLeft = true;

    private void Start()
    {
        EnemiesManager.NoEnemiesLeft += OnNoEnemiesLeft;
    }

    private void OnDisable()
    {
        EnemiesManager.NoEnemiesLeft -= OnNoEnemiesLeft;
    }

    private void OnNoEnemiesLeft()
    {
        _enemiesLeft = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_isEnding)
        {
            return;
        }
        if(other.CompareTag("Player"))
        {
            if(TryEndLevel())
            {
                _isEnding = true;
                other.GetComponent<PlayerCharacter>().DisableInput();
            }
        }
    }

    private bool TryEndLevel()
    {
        if(!_enemiesLeft)
        {
            AnimateDoorClose();
            return true;
        }
        return false;
    }

    private void AnimateDoorClose()
    {
        StartCoroutine(ScaleDoor());
    }

    private IEnumerator ScaleDoor()
    {
        Vector3 targetScale = new Vector3(1, _elevatorDoor.localScale.y, _elevatorDoor.localScale.z);
        float duration = 3f;
        float time = 0f;

        while(time < duration)
        {
            _elevatorDoor.localScale = Vector3.Lerp(_elevatorDoor.localScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        EndLevel();
    }
    
    private void EndLevel()
    {
        LevelController.Instance.CompleteLevel();
    }
}
