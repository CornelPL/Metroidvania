using UnityEngine;
using UnityEngine.Events;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private UnityEvent OnDestroy = null;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerHealthManager>().TakeDamage(damage, transform.position.x);
        }

        OnDestroy.Invoke();

        Destroy(gameObject);
    }
}
