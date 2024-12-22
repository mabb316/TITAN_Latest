using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public static event Action<bool, WeaponBase> WeaponStatusChanged;

    [SerializeField] protected float _attackCooldown;
    [SerializeField] protected int _maxAmmo;
    [SerializeField] protected float _bulletSpeed;
    [SerializeField] protected GameObject _bulletPrefab;
    [SerializeField] protected Transform _shootingPoint;
    [SerializeField] private WeaponType _weaponType;
    [SerializeField] private AudioClip _shootSound;

    protected int _remainingAmmo;
    protected bool _isHeldByPlayer = false;
    
    private bool _hasOwner = false;
    private bool _isReadyToShoot = true;
    private bool _isBeingThrown = false;
    
    public bool IsFree => !_hasOwner && _remainingAmmo > 0 && !_isBeingThrown;

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        _remainingAmmo = _maxAmmo;
    }

    public void Shoot(Vector3 directionToShoot)
    {
        if(!_isReadyToShoot)
        {
            return;
        }

        StartCoroutine(ShootingCooldown());

        ShootAction(directionToShoot);
    }

    protected virtual void ShootAction(Vector3 directionToShoot)
    {
        UseAmmo();

        GameObject bulletObject = Instantiate(_bulletPrefab, _shootingPoint.position, Quaternion.identity);

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        SetBulletProperties(bullet, directionToShoot);

        AudioSource.PlayClipAtPoint(_shootSound, Camera.main.transform.position);
    }

    protected void SetBulletProperties(Bullet bullet, Vector3 directionToShoot)
    {
        bullet.SetShotByPlayer(_isHeldByPlayer);

        bullet.Initialize(directionToShoot, _bulletSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_isBeingThrown)
            return;

        bool stopFlag = false;

        if(other.CompareTag("Enemy"))
        {
            if(other.TryGetComponent<CharacterBase>(out CharacterBase character))
            {
                character.TakeDamage(1, Vector3.zero);
                stopFlag = true;
            }
        }
        else if(other.CompareTag("Wall"))
        {
            stopFlag = true;
        }

        if(!stopFlag)
            return;
        
        _isBeingThrown = false;

        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;
    }

    private IEnumerator ShootingCooldown()
    {
        _isReadyToShoot = false;
        yield return new WaitForSeconds(_attackCooldown);
        _isReadyToShoot = true;
    }

    protected void UseAmmo(int ammoToUse = 1)
    {
        _remainingAmmo -= ammoToUse;
    }

    public int GetRemainingAmmo()
    {
        return _remainingAmmo;
    }

    public int GetMaxAmmo()
    {
        return _maxAmmo;
    }

    public void SetHasOwner(bool hasOwner)
    {
        WeaponStatusChanged?.Invoke(IsFree, this);
        
        _hasOwner = hasOwner;
    }

    public void SetHeldByPlayer(bool heldByPlayer)
    {
        _isHeldByPlayer = heldByPlayer;
    }

    public bool GetHasOwner()
    {
        return _hasOwner;
    }

    public void SetIsBeingThrown(bool isBeingThrown)
    {
        _isBeingThrown = isBeingThrown;
    }

    public WeaponType GetWeaponType()
    {
        return _weaponType;
    }

    public bool CanBePickedUp()
    {
        return !_isBeingThrown && !_hasOwner && _remainingAmmo > 0;
    }

}
