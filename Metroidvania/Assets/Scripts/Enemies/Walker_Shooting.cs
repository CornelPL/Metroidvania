using UnityEngine;

public class Walker_Shooting : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private int projectilesCount = 3;
    [SerializeField] private float projectilesForce = 100f;
    [SerializeField] private GameObject projectile = null;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private EnemyHealthManager healthManager = null;

    private int direction = 1;
    private float timeWalkingTooSlow = 0f;
    private bool playerInRange = false;
    private bool isShooting = false;


    private void Update()
    {
        if (!healthManager.isBeingKnockbacked && !isShooting)
        {
            CheckPlayerInSight();
            if (playerInRange)
            {
                Invoke("Shoot", 0.5f);
                isShooting = true;
            }
            else
            {
                Move();
            }
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


    private void Shoot()
    {
        // TODO: play animation and fire projectiles from animation
        ShootProjectiles();
    }


    public void ShootProjectiles()
    {
        Vector2 dir;
        for (int i = 0; i < projectilesCount; i++)
        {
            dir = Random.insideUnitCircle;
            dir.y = Mathf.Abs(dir.y) + 1f;
            dir.Normalize();

            GameObject proj = Instantiate(projectile, transform.position, transform.rotation);
            proj.GetComponent<Rigidbody2D>().AddForce(dir * projectilesForce, ForceMode2D.Impulse);
        }
        isShooting = false;
    }


    private void CheckPlayerInSight()
    {
        if (Physics2D.OverlapCircle(transform.position, sightRange, playerLayerMask))
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
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
        else if (healthManager.isBeingKnockbacked && !collision.gameObject.CompareTag("Item"))
        {
            healthManager.isBeingKnockbacked = false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, Vector2.up * sightRange);
        Gizmos.DrawRay(transform.position, Vector2.left * sightRange);
        Gizmos.DrawRay(transform.position, Vector2.right * sightRange);
    }
}
