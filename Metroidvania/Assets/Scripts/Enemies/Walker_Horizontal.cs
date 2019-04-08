using UnityEngine;

public class Walker_Horizontal : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private int damage = 20;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private EnemyHealthManager healthManager = null;

    private int direction = 1;


    private void FixedUpdate()
    {
        if (Mathf.Abs(_rigidbody.velocity.x) < minSpeed)
        {
            ChangeDirection();
        }

        if (!healthManager.isBeingKnockbacked)
        {
            _rigidbody.velocity = new Vector2(speed * direction, _rigidbody.velocity.y);
        }
    }


    private void ChangeDirection()
    {
        direction = direction > 0 ? -1 : 1;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("StopMark"))
        {
            ChangeDirection();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerHealthManager>().TakeDamage(damage, transform.position.x);
        }
    }
}
