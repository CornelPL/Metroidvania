using UnityEngine;

public class Walker_Horizontal : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private int damage = 20;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private EnemyHealthManager healthManager = null;
    [Tooltip("1 - right; -1 - left")]
    [SerializeField] private int direction = 1;

    private float timeWalkingTooSlow = 0f;


    private void Update()
    {
        if (!healthManager.isBeingKnockbacked)
        {
            Move();
        }
    }

    private void Move()
    {
        // TODO: Change moving detection in all walkers to collision, not actual speed
        if (Mathf.Abs(_rigidbody.velocity.x) < minSpeed)
        {
            timeWalkingTooSlow += Time.fixedDeltaTime;
        }
        if (timeWalkingTooSlow > 0.1f)
        {
            ChangeDirection();
            timeWalkingTooSlow = 0f;
        }
        _rigidbody.velocity = new Vector2(speed * direction, _rigidbody.velocity.y);
    }

    private void ChangeDirection()
    {
        direction = direction > 0 ? -1 : 1;
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ( collider.gameObject.CompareTag("StopMark"))
        {
            ChangeDirection();
        }
        else if ( collider.gameObject.CompareTag( "Player" ) )
        {
            collider.GetComponent<PlayerHealthManager>().TakeDamage( damage, transform.position.x );
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (healthManager.isBeingKnockbacked)
        {
            healthManager.isBeingKnockbacked = false;
        }
    }
}
