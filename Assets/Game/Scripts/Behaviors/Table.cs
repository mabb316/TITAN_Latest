using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// kullanılmıyor
public class Table : MonoBehaviour, IInteractable, IDamageable
{
    private bool _isFlipped = false;
    [SerializeField] private int _health = 3;
    private Rigidbody _rigidbody;
    private bool _isSliding => _rigidbody.linearVelocity.sqrMagnitude > .1f;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Interact(GameObject interactSource)
    {
        Flip(interactSource.transform);
    }

    private void Flip(Transform interactTransform)
    {
        if (_isFlipped)
            return;

        Vector3 direction = (transform.position - interactTransform.position).normalized;
        Vector3 cross = Vector3.Cross(Vector3.up, direction);

        Quaternion rotation = Quaternion.AngleAxis(-90, cross);

        StartCoroutine(AnimateFlip(rotation));

        _isFlipped = true;
    }

    private IEnumerator AnimateFlip(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public void Push(GameObject interactSource)
    {
        if (!_isFlipped)
            return;

        Vector3 direction = (transform.position - interactSource.transform.position).normalized;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            transform.Translate(Vector3.right * 2, Space.World);
        }
        else
        {
            transform.Translate(Vector3.forward * 2, Space.World);
        }
    }

    public void FlingTable(Vector3 direction)
    {
        if (!_isFlipped)
            return;

        direction.Normalize();

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.AddForce(direction * 500, ForceMode.Impulse);


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(1, _rigidbody.linearVelocity.normalized);
            }
        }
    }

    public void TakeDamage(int damage, Vector3 direction)
    {
        if (!_isFlipped)
            return;

        _health -= damage;

        if(_health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
