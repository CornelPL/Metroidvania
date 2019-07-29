using UnityEngine;

public class Charger : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private float chargeSpeed = 6f;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private EnemyHealthManager healthManager = null;

    private int direction = 1;
    private float timeWalkingTooSlow = 0f;
    private bool isCharging = false;
    private bool isStunned = false;
    private float timeStunned = 0f;


    private void Update()
    {
        if (!isStunned)
        {
            if (!healthManager.isBeingKnockbacked && !isCharging)
            {
                CheckPlayerInSight();
                Move();
            }
            else if (isCharging)
            {
                Charge();
            }
        }
        else if (timeStunned < stunTime)
        {
            // Play stunned anim
            timeStunned += Time.deltaTime;
        }
        else
        {
            timeStunned = 0f;
            isStunned = false;
            ChangeDirection();
        }
    }


    private void Move()
    {
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


    private void CheckPlayerInSight()
    {
        if (Physics2D.Raycast(transform.position, new Vector2(direction, 0f), sightRange, playerLayerMask))
        {
            isCharging = true;
            Charge();
        }
    }


    private void Charge()
    {
        _rigidbody.velocity = new Vector2(chargeSpeed * direction, _rigidbody.velocity.y);
    }


    private void ChangeDirection()
    {
        direction = direction > 0 ? -1 : 1;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("StopMark") && !isCharging)
        {
            ChangeDirection();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerHealthManager>().TakeDamage(damage, transform.position.x);

            if (isCharging) isCharging = false;
        }
        else if (isCharging)
        {
            isStunned = true;
        }
        else if (healthManager.isBeingKnockbacked && !collision.gameObject.CompareTag("Item"))
        {
            healthManager.isBeingKnockbacked = false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, new Vector2(direction * sightRange, 0f));
    }
}
