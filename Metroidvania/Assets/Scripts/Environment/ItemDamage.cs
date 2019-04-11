using UnityEngine;

public class ItemDamage : MonoBehaviour
{
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private Rigidbody2D rb = null;
    public bool isShooted = false;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isShooted)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                int damage = (int)(baseDamage * rb.velocity.magnitude / 10 * rb.mass);
                collision.gameObject.GetComponent<EnemyHealthManager>().TakeDamage(damage, transform.position.x);
            }

            // Add better destroy
            Destroy(gameObject);
        }
    }
}