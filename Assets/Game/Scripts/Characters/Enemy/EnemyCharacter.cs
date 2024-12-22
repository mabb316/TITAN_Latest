using UnityEngine.AI;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyCharacter : CharacterBase
{
    public event Action<EnemyCharacter> Died;
    private bool _isAlerted = false;
    private Transform _player;
    private Coroutine _thinkRoutine = null;
    private bool _isAIDisabled = false;

    [SerializeField]
    private List<string> _obstructionsTags;

    private void Update()
    {
        if (!_isAlive || _isAIDisabled)
        {
            return;
        }

        if(_isAlerted && _heldWeapon != null)
        {
            LookAtPlayer();
        }
    }

    protected override void Initialize()
    {
        base.Initialize();

        PlayerCharacter.PlayerInitialized += OnPlayerInitialized;

        _thinkRoutine = StartCoroutine(ThinkRoutine());
    }

    private void OnPlayerInitialized(Transform player)
    {
        _player = player;

        PlayerCharacter.PlayerInitialized -= OnPlayerInitialized;
    }

    protected override void OnDied()
    {
        base.OnDied();

        Died?.Invoke(this);

        if(_thinkRoutine != null)
        {
            StopCoroutine(_thinkRoutine);
        }

        (_characterMovementController as EnemyMovementController).SetIsMoving(false);
        _characterMovementController.SetCanMove(false);
    }

    protected override void OnWeaponPickedUp()
    {
        base.OnWeaponPickedUp();

        _heldWeapon.SetHeldByPlayer(false);
    }

    private void HandleWithWeapon()
    {
        if (HasLineOfSight())
        {
            (_characterMovementController as EnemyMovementController).SetRotationOverrid(true);
            ShootAtPlayer();
            Vector3 randomDirection = GetRandomDirection();
            (_characterMovementController as EnemyMovementController).MoveToPosition(transform.position + randomDirection * 5f);
        }
        else
        {
            (_characterMovementController as EnemyMovementController).SetRotationOverrid(false);

            (_characterMovementController as EnemyMovementController).MoveToPosition(GetPlayerTransform().position);
        }
    }

    private void HandleWithoutWeapon()
    {
        (_characterMovementController as EnemyMovementController).SetRotationOverrid(false);
        Transform nearestWeapon = SceneWeaponsController.Instance.GetNearestFreeWeaponTransform(transform.position);
        if (nearestWeapon != null)
        {
            (_characterMovementController as EnemyMovementController).MoveToPosition(nearestWeapon.position);
        }
        else
        {
            Vector3 directionAwayFromPlayer = transform.position - GetPlayerTransform().position;
            (_characterMovementController as EnemyMovementController).MoveToPosition(transform.position + directionAwayFromPlayer);
            _characterAnimatorController.SetIsTerrified(true);
        }
    }

    private bool HasLineOfSight()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, GetPlayerTransform().position - transform.position);
        bool hitPlayer = false;
        foreach (var hit in hits)
        {
            if (_obstructionsTags.Contains(hit.transform.tag))
            {
                return false;
            }
            if (hit.transform == GetPlayerTransform())
            {
                hitPlayer = true;
            }
        }
        return hitPlayer;
    }

    private void ShootAtPlayer()
    {
        Vector3 directionToPlayer = (GetPlayerTransform().position - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, directionToPlayer, Time.deltaTime * 2f);

        if (Vector3.Angle(transform.forward, directionToPlayer) < 20f)
        {
            Attack(transform.forward);
        }
    }

    private Vector3 GetRandomDirection()
    {
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        return directions[UnityEngine.Random.Range(0, directions.Length)];
    }

    protected override void OnHandEmpty()
    {
        base.OnHandEmpty();

        DropWeapon();
    }

    private IEnumerator ThinkRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(.5f);

            if (!_isAlerted)
            {
                continue;
            }

            if (_heldWeapon != null)
            {
                HandleWithWeapon();
            }
            else
            {
                HandleWithoutWeapon();
            }
        }
    }

    private void LookAtPlayer()
    {
        Vector3 directionToPlayer = (GetPlayerTransform().position - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, directionToPlayer, Time.deltaTime * 2f);
    }

    public void Alert()
    {
        _isAlerted = true;
    }

    public void DisableAI()
    {
        StopCoroutine(_thinkRoutine);
        _characterMovementController.SetCanMove(false);
        (_characterMovementController as EnemyMovementController).SetIsMoving(false);
        _isAIDisabled = true;
    }

    protected override void OnRanOutOfAmmo()
    {
        base.OnRanOutOfAmmo();

        DropWeapon();
    }

    private Transform GetPlayerTransform()
    {
        if(_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        return _player;
    }
}