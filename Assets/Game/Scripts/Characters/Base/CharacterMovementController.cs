using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    protected Transform _characterModel;
    private CharacterAnimatorController _characterAnimator;
    protected float _movementSpeed = 0f;
    protected bool _canMove = true;
    protected bool _isMoving = false;

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        _characterAnimator = GetComponent<CharacterAnimatorController>();
    }

    protected void HandleAnimation()
    {
        if(_characterAnimator == null)
            return;
        
        _characterAnimator.SetWalkingAnimation(_isMoving);
    }

    public void SetCharacterModel(Transform characterModel)
    {
        _characterModel = characterModel;
    }

    public void SetMovementSpeed(float movementSpeed)
    {
        _movementSpeed = movementSpeed;
    }

    public void SetCanMove(bool canMove)
    {
        _canMove = canMove;

        if(!canMove && _characterAnimator != null)
            _characterAnimator.SetWalkingAnimation(false);
    }

    private void SetIsMoving(bool isMoving)
    {
        _isMoving = isMoving;
    }
}
