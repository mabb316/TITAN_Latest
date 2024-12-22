using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour, IDamageable
{
    [SerializeField] protected Transform _characterModel;
    [SerializeField] protected WeaponBase _startingWeapon;
    [SerializeField] private CharacterSO _characterSO;
    [SerializeField] protected GameObject _characterRagdoll;
    [SerializeField] protected CharacterAnimatorController _characterAnimatorController;
    [SerializeField] protected CharacterMovementController _characterMovementController;
    [SerializeField] private Transform _weaponHoldPosition;
    [SerializeField] protected Animator _animator;

    private Vector3 _storedForceDirection;

    private float _movementSpeed;

    protected bool _isAlive = true;

    protected WeaponBase _heldWeapon;

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        _movementSpeed = _characterSO.MovementSpeed;

        _characterMovementController.SetMovementSpeed(_movementSpeed);
        _characterMovementController.SetCharacterModel(_characterModel);

        _characterAnimatorController.SetAnimator(_animator);
        
        if(_startingWeapon)
        {
            PickUpWeapon(_startingWeapon, true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon"))
        {
            WeaponBase weaponOnGround = other.GetComponent<WeaponBase>();
            PickUpWeapon(weaponOnGround);
        }
    }

    private void PickUpWeapon(WeaponBase weapon, bool isForcePickup = false)
    {
        if(!CanPickupWeapon(weapon) && !isForcePickup)
        {
            return;
        }

        _heldWeapon = weapon;
        weapon.SetHasOwner(true);
        weapon.transform.parent = _weaponHoldPosition;
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        OnWeaponPickedUp();
    }

    protected virtual bool CanPickupWeapon(WeaponBase weapon)
    {
        return !_heldWeapon && _isAlive && weapon.CanBePickedUp();
    }

    protected virtual void OnWeaponPickedUp()
    {
        _characterAnimatorController.SetHasWeapon(true);
    }

    protected void Attack(Vector3 attackDirection)
    {
        if(!_isAlive)
        {
            return;
        }
        if(!_heldWeapon)
        {
            OnHandEmpty();
            return;
        }
        if(_heldWeapon.GetRemainingAmmo() == 0)
        {
            OnRanOutOfAmmo();
            return;
        }

        _heldWeapon.Shoot(attackDirection);

        OnAttacked();
    }

    protected virtual void OnAttacked()
    {

    }

    protected virtual void OnHandEmpty()
    {

    }

    protected virtual void OnRanOutOfAmmo()
    {

    }
    
    public virtual void TakeDamage(int damage, Vector3 direction)
    {
        if(!_isAlive)
            return;

        Die();

       _storedForceDirection = direction;
    }

    public virtual void Die()
    {
        _isAlive = false;
        _characterMovementController.SetCanMove(false);

        OnDied();
    }

    protected virtual void OnDied()
    {
        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        if(_heldWeapon)
        {
            DropWeapon();
        }

        SwitchToRagdoll();
    }

    protected virtual void DropWeapon()
    {
        if(!_heldWeapon)
            return;

        _heldWeapon.transform.parent = LevelController.Instance.CurrentLevelTransform;
        
        _heldWeapon.SetHasOwner(false);
        _heldWeapon.SetHeldByPlayer(false);
        if(_heldWeapon.GetRemainingAmmo() <= 0)
        {
            Rigidbody rigidbody = _heldWeapon.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.useGravity = true;
        }
        _heldWeapon = null;
        
        OnLostWeapon();
    }

    protected virtual void OnLostWeapon()
    {
        _characterAnimatorController.SetHasWeapon(false);
    }

    private void SwitchToRagdoll()
    {
        _characterModel.gameObject.SetActive(false);
        _characterRagdoll.SetActive(true);
        if(TryGetComponent<Rigidbody>(out Rigidbody ragdollRb))
        {
            ragdollRb.isKinematic = false;
            ragdollRb.useGravity = true;
            ragdollRb.AddForce(_storedForceDirection * 10, ForceMode.Impulse);
        }
    }
}
