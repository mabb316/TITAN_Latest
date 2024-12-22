using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : CharacterMovementController
{
    private CharacterController _characterController;
    private TrailRenderer _trailRenderer;

    private void Update()
    {
        if(!_canMove)
        {
            return;
        }
        
        if (GetCanMove())
        {
            HandleMovement();
        }

        if (GetCanRotate())
        {
            HandleRotation();
        }

        HandleAnimation();
    }

    protected override void Initialize()
    {
        base.Initialize();

        _characterController = GetComponent<CharacterController>();
        _trailRenderer = GetComponent<TrailRenderer>();
        if (_trailRenderer != null)
        {
            _trailRenderer.emitting = false;
        }
    }

    private void HandleMovement()
    {
        Vector3 movementDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (movementDir.magnitude > 0)
        {
            _isMoving = true;
        }
        else
        {
            _isMoving = false;
        }

        _characterController.Move(movementDir * _movementSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector3 cursorPosition = MouseWorldPositionController.Instance.GetCursorPosTransform().position;
        Vector3 direction = cursorPosition - _characterModel.transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _characterModel.transform.rotation = targetRotation;
        }
    }

    private bool GetCanMove()
    {
        return _canMove && _characterController != null;
    }

    private bool GetCanRotate()
    {
        return _characterModel != null;
    }

    public void Dash()
    {
        if (_trailRenderer != null)
        {
            StartCoroutine(DashCoroutine());
        }
        else
        {
            PerformDash();
        }
    }

    private IEnumerator DashCoroutine()
    {
        _trailRenderer.emitting = true;
        PerformDash();
        yield return new WaitForSeconds(0.5f); // Adjust the duration as needed
        _trailRenderer.emitting = false;
    }

    private void PerformDash()
    {
        Vector3 dashDirection = _characterController.velocity.normalized;
        _characterController.Move(dashDirection * 2.5f);
    }
}
