using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private ParticleSystem _confettiParticles;
    [SerializeField] private GameObject _enemyModel;

    private bool _isDead = false;
    public void TakeDamage(int damage, Vector3 direction)
    {
        if (_isDead)
        {
            return;
        }

        Die();
    }

    public void Die()
    {
        _enemyModel.SetActive(false);
        _confettiParticles.Play();
        _isDead = true;
        LevelController.Instance.CompleteLevel();
    }
}
