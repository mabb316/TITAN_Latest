using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCharacter : CharacterBase
{
    public static event Action<Transform> PlayerInitialized;
    public static event Action PlayerDied;
    public static event Action<WeaponType> WeaponReceived;
    public static event Action WeaponLost;
    public static event Action<int, int> BulletCountChanged;

    [SerializeField] private PlayerMovementController _playerMovementContoller;
    [SerializeField] private AudioClip _dashSound;

    private float _throwHoldTime = .5f;
    private float _holdTime = 0;

    private float _kickCooldown = 2f;
    private bool _kickReady = true;

    private float _dashCooldown = 2f;
    private bool _dashReady = true;

    private bool _isInvincible = false;
    private float _invincibilityTime = .5f;

    private bool _isInputDisabled = false;

    protected override void Initialize()
    {
        base.Initialize();

        PlayerInitialized?.Invoke(transform);
    }

    private void Update()
    {
        if(!_isAlive || _isInputDisabled)
        {
            return;
        }
        HandleInputs();
    }

    private void HandleInputs()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Attack(_characterModel.forward.normalized);
        }
        if(Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            Kick();
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryDash();
        }
        if(Input.GetKey(KeyCode.G))
        {
            _holdTime += Time.deltaTime;
            if(_holdTime >= _throwHoldTime)
            {
                ThrowWeapon();
            }
        }
        if(Input.GetKeyUp(KeyCode.G))
        {
            DropWeapon();
        }
    }

    protected override void OnWeaponPickedUp()
    {
        base.OnWeaponPickedUp();

        _heldWeapon.SetHeldByPlayer(true);

        WeaponReceived?.Invoke(_heldWeapon.GetWeaponType());

        BulletCountChanged?.Invoke(_heldWeapon.GetRemainingAmmo(), _heldWeapon.GetMaxAmmo());
    }

    protected override void OnHandEmpty()
    {
        Kick();
    }

    protected override void OnRanOutOfAmmo()
    {
        ThrowWeapon();
    }

    private void Kick()
    {
        if(!_kickReady)
            return;

        KickCooldown();

        _characterAnimatorController.PlayKickAnimation();
        
        if(Physics.Raycast(_characterModel.transform.position, _characterModel.transform.forward, out RaycastHit raycastHit, 3f))
        {
            Collider collider = raycastHit.collider;
            if(collider.CompareTag("Enemy"))
            {
                if(collider.TryGetComponent<CharacterBase>(out CharacterBase character))
                {
                    character.TakeDamage(1, _characterModel.transform.forward);
                }
                else if(collider.TryGetComponent<BossEnemy>(out BossEnemy bossEnemy))
                {
                    bossEnemy.TakeDamage(1, _characterModel.transform.forward);
                }
            }
            else if(collider.CompareTag("Door"))
            {
                Door door = collider.GetComponent<Door>();
                door.BreakDoor(_characterModel.transform.forward);
            }
            else if(collider.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.AddForce(_characterModel.transform.forward * 20, ForceMode.Impulse);
            }
        }
    }

    private void TryDash()
    {
        if(!_dashReady)
            return;

        StartCoroutine(InvincibilityCooldown());
        StartCoroutine(DashCooldown());

        _playerMovementContoller.Dash();
        AudioSource.PlayClipAtPoint(_dashSound, Camera.main.transform.position);
    }

    private IEnumerator DashCooldown()
    {
        _dashReady = false;
        yield return new WaitForSeconds(_dashCooldown);
        _dashReady = true;
    }

    private IEnumerator InvincibilityCooldown()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(_invincibilityTime);
        _isInvincible = false;
    }

    private void Interact()
    {
        RaycastHit raycastHit;

        if(Physics.Raycast(_characterModel.position, _characterModel.forward, out raycastHit, 3f))
        {
            if(raycastHit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.Interact(gameObject);
                return;
            }
        }
    }

    private void ThrowWeapon()
    {
        if(!_heldWeapon)
        {
            return;
        }
        Rigidbody rigidbody = _heldWeapon.GetComponent<Rigidbody>();
        _heldWeapon.transform.parent = LevelController.Instance.CurrentLevelTransform;
        _heldWeapon.SetIsBeingThrown(true);
        _heldWeapon.SetHasOwner(false);
        _heldWeapon = null;

        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.AddForce(_characterModel.forward * 10, ForceMode.Impulse);

        OnLostWeapon();
    }

    private IEnumerator KickCooldown()
    {
        _kickReady = false;
        yield return new WaitForSeconds(_kickCooldown);
        _kickReady = true;
    }

    public override void TakeDamage(int damage, Vector3 direction)
    {
        if(!_isAlive)
            return;
        if(_isInvincible)
            return;

        base.TakeDamage(damage, direction);
    }

    protected override void OnDied()
    {
        base.OnDied();

        _playerMovementContoller.SetCanMove(false);

        PlayerDied?.Invoke();
    }

    public void StopMovement()
    {
        _playerMovementContoller.SetCanMove(false);
    }

    protected override void OnAttacked()
    {
        base.OnAttacked();

        if(_heldWeapon)
        {
            BulletCountChanged?.Invoke(_heldWeapon.GetRemainingAmmo(), _heldWeapon.GetMaxAmmo());
        }

        AlarmManager.Instance.InvokeAlarm();
    }

    protected override void OnLostWeapon()
    {
        base.OnLostWeapon();

        WeaponLost?.Invoke();
    }

    public void DisableInput()
    {
        _playerMovementContoller.SetCanMove(false);

        _isInputDisabled = true;
    }
}
