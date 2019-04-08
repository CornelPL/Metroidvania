using UnityEngine;

public class ItemDamage : MonoBehaviour
{
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float minSpeedToDamage = 10f;
    [SerializeField] private Rigidbody2D rb = null;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && rb.velocity.magnitude > minSpeedToDamage)
        {
            int damage = (int)(baseDamage * rb.velocity.magnitude / 10 * rb.mass);
            collision.gameObject.GetComponent<EnemyHealthManager>().TakeDamage(damage, transform.position.x);
        }
    }
}