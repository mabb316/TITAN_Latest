using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorController : MonoBehaviour
{
    private Animator _characterAnimator;

    private bool _isAnimatorValid => _characterAnimator != null;

    public void SetAnimator(Animator animator)
    {
        _characterAnimator = animator;
    }

    public void SetWalkingAnimation(bool isWalking)
    {
        if (!_isAnimatorValid)
        {
            return;
        }

        _characterAnimator.SetBool("Walking", isWalking);
    }

    public void PlayKickAnimation()
    {
        if (!_isAnimatorValid)
        {
            return;
        }
        _characterAnimator.SetTrigger("Kick");
    }

    public void SetHasWeapon(bool hasWeapon)
    {
        if (!_isAnimatorValid)
        {
            return;
        }
        _characterAnimator.SetBool("HasWeapon", hasWeapon);
    }

    public void SetIsTerrified(bool isTerrified)
    {
        if (!_isAnimatorValid)
        {
            return;
        }
        _characterAnimator.SetBool("IsTerrified", isTerrified);
    }

}
