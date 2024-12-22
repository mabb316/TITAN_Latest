using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInner : MonoBehaviour
{
    private bool _isFlying => _rigidbody.linearVelocity.sqrMagnitude > .5f;
    private Rigidbody _rigidbody;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void FlingDoor(Vector3 direction)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.AddForce(direction * 20, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if(!_isFlying)
            {
                return;
            }
            CharacterBase character = other.GetComponent<CharacterBase>();
            character.TakeDamage(3, _rigidbody.linearVelocity.normalized);
        }
    }
}
