using UnityEngine;
using UnityEngine.Events;

public class Enemy_Explosive_Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 15;
    [SerializeField] private float timeToExplode = 2f;
    [SerializeField] private float range = 2f;
    [SerializeField] private LayerMask playerMask = 0;
    [SerializeField] private UnityEvent OnCollision = null;
    [SerializeField] private UnityEvent OnDestroy = null;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision.Invoke();

        Invoke("Explode", timeToExplode);
    }


    private void Explode()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, range, playerMask);
        if (hit)
        {
            hit.GetComponent<PlayerHealthManager>().TakeDamage(damage, transform.position.x);
        }

        OnDestroy.Invoke();

        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, Vector2.up * range);
        Gizmos.DrawRay(transform.position, Vector2.down * range);
        Gizmos.DrawRay(transform.position, Vector2.left * range);
        Gizmos.DrawRay(transform.position, Vector2.right * range);
    }
}
