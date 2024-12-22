using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementController : CharacterMovementController
{
    private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        Initialize();
    }

    protected override void Initialize()
    {
        base.Initialize();

        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void MoveToPosition(Vector3 destination)
    {
        if(!_canMove)
        {
            _isMoving = false;
            return;
        }

        _navMeshAgent.SetDestination(destination);
        _isMoving = true;

        HandleAnimation();
    }

    public void SetRotationOverrid(bool isOverrid)
    {
        _navMeshAgent.updateRotation = !isOverrid;
    }

    public void SetIsMoving(bool isMoving)
    {
        _navMeshAgent.isStopped = !isMoving;
    }
}
