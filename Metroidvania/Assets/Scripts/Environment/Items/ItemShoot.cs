using UnityEngine;

public class ItemShoot : MonoBehaviour
{
    public enum ItemType
    {
        rock,
        plank,
        barrel,
        saw
    }

    [SerializeField] ItemType itemType = ItemType.rock;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private Rigidbody2D rb = null;
    public bool isShooted = false;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isShooted)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                DoDamage(collision.gameObject);
                CustomDestroy();
            }

            if (itemType == ItemType.plank)
            {
                if (collision.gameObject.CompareTag("Wall"))
                {
                    rb.velocity = Vector2.zero;
                    rb.simulated = false;
                    gameObject.layer = LayerMask.NameToLayer("Ground");
                }
            }
        }
    }


    private void DoDamage(GameObject go)
    {
        int damage = (int)(baseDamage * rb.velocity.magnitude / 10 * rb.mass);
        go.GetComponent<EnemyHealthManager>().TakeDamage(damage, transform.position.x);
    }


    private void CustomDestroy()
    {
        Destroy(gameObject);
    }
}