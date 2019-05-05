using UnityEngine;

public class FallingSpikes : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float gravityScale = 8f;
    [SerializeField] private Rigidbody2D _rigidbody = null;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _rigidbody.gravityScale = gravityScale;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealthManager>().TakeDamage(damage, transform.position.x);
        }

        Destroy(gameObject);
    }
}
