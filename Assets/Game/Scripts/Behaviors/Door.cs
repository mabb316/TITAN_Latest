using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private DoorInner _doorInner;
    [SerializeField] private AudioClip _doorOpenSound;
    [SerializeField] private AudioClip _doorBreakSound;

    private bool _isBroken = false;
    private bool _isOpen;

    public void Interact(GameObject interactSource)
    {
        if(_isBroken)
            return;
        if(!_isOpen)
        {
            OpenDoor(interactSource.transform);
        }
        else
            CloseDoor();
        
        AudioSource.PlayClipAtPoint(_doorOpenSound, Camera.main.transform.position);
    }

    private void OpenDoor(Transform interactTransform)
    {
        Vector3 playerPosition = interactTransform.position;
        Vector3 doorPosition = transform.position;
        Vector3 directionToPlayer = (playerPosition - doorPosition).normalized;

        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

        Quaternion targetRotation;
        if (dotProduct > 0)
        {
            targetRotation = transform.localRotation * Quaternion.Euler(0, 90, 0);
        }
        else
        {
            targetRotation = transform.localRotation * Quaternion.Euler(0, -90, 0);
        }

        StartCoroutine(RotateDoor(transform, targetRotation, 1.0f));
        _isOpen = true;
    }

    private void CloseDoor()
    {
        Quaternion targetRotation = Quaternion.Euler(0, 0, 0);
        StartCoroutine(RotateDoor(transform, targetRotation, 1.0f));
        _isOpen = false;
    }

    private IEnumerator RotateDoor(Transform door, Quaternion targetRotation, float duration)
    {
        Quaternion initialRotation = door.rotation;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            door.rotation = Quaternion.Slerp(initialRotation, targetRotation, time / duration);
            yield return null;
        }

        door.rotation = targetRotation;
    }


    public void BreakDoor(Vector3 direction)
    {
        if(_isBroken)
            return;

        _isBroken = true;
        _doorInner.transform.parent = null;
        _doorInner.FlingDoor(direction);

        AudioSource.PlayClipAtPoint(_doorBreakSound, Camera.main.transform.position);

        StartCoroutine(DoorRemoveRoutine());
    }

    private IEnumerator DoorRemoveRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        if(_doorInner != null)
            Destroy(_doorInner.gameObject);
    }
}
