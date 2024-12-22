using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    private Vector3 moveDirection;
    private float speed;
    private bool _shotByPlayer = false;
    private bool _isInitialized = false;
    private TrailRenderer _trailRenderer;

    protected virtual int Damage => 1;

    private void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    public void Initialize(Vector3 direction, float bulletSpeed)
    {
        moveDirection = direction;
        speed = bulletSpeed;

        _isInitialized = true;

        StartCoroutine(BulletLifetime());
    }

    private IEnumerator BulletLifetime()
    {
        yield return new WaitForSeconds(4);

        if(gameObject == null)
        {
            yield break;
        }

        Destroy(gameObject);
    }

    public void SetShotByPlayer(bool shotByPlayer)
    {
        _shotByPlayer = shotByPlayer;
    }

    private void Update()
    {
        if(!_isInitialized)
        {
            return;
        }

        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool destroyFlag = false;

        if(!_shotByPlayer)
        {
            if(other.CompareTag("Player"))
            {
                if(other.TryGetComponent<CharacterBase>(out CharacterBase playerCharacter))
                {
                    playerCharacter.TakeDamage(Damage, moveDirection);
                    destroyFlag = true;
                }
            }
            else if(other.CompareTag("Bullet"))
            {
                destroyFlag = true;
            }
        }
        else
        {
            if(other.CompareTag("Enemy"))
            {
                if(other.TryGetComponent<CharacterBase>(out CharacterBase enemyCharacter))
                {
                    enemyCharacter.TakeDamage(Damage, moveDirection);
                    destroyFlag = true;
                }
                else if(other.TryGetComponent<BossEnemy>(out BossEnemy bossEnemy))
                {
                    bossEnemy.TakeDamage(Damage, moveDirection);
                    destroyFlag = true;
                }
            }
        }
        if(!destroyFlag)
        {
            if(other.CompareTag("Wall"))
            {
                destroyFlag = true;
            }
        }

        OnCollidedWithSomething(destroyFlag);
    }

    protected virtual void OnCollidedWithSomething(bool destroyFlag)
    {
        if(destroyFlag)
        {
            _trailRenderer.transform.parent = null;
            _trailRenderer.autodestruct = true;

            Destroy(gameObject);
        }
    }

}
