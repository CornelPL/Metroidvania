using UnityEngine;

public class Walker_Horizontal : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private int damage = 20;

    private Rigidbody2D _rigidbody;
    private int direction = 1;
    private Vector2 previousPosition;
    private Vector2 currentPosition;
    private bool isBeingKnockbacked = false;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }


    private void FixedUpdate()
    {
        previousPosition = transform.position;
        if (Mathf.Abs(_rigidbody.velocity.x) < minSpeed)
        {
            ChangeDirection();
        }

        if (!isBeingKnockbacked)
        {
            _rigidbody.velocity = new Vector2(speed * direction, _rigidbody.velocity.y);
        }

        currentPosition = transform.position;
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
            collision.collider.GetComponent<HealthManager>().TakeDamage(damage);
        }
    }
}
