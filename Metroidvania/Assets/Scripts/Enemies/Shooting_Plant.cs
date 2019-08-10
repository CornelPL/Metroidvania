using UnityEngine;

public class Shooting_Plant : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float range = 5f;
    [SerializeField] private float timeBetweenShots = 1f;
    [SerializeField] private float shootForce = 100f;
    [SerializeField] private GameObject projectile = null;
    [SerializeField] private int projectilesCount = 1;
    [SerializeField] private float spread = 10f;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField] private Vector2 aimOffset = Vector2.zero;

    private bool playerInRange = false;
    private float timeAfterLastShot = 0f;
    private Transform player = null;


    private void Update()
    {
        CheckPlayerInRange();

        if (playerInRange)
        {
            if (timeAfterLastShot > timeBetweenShots)
            {
                Vector2 direction = (Vector2)player.position + aimOffset - (Vector2)transform.position;
                Shoot(direction);
            }
            else
            {
                timeAfterLastShot += Time.deltaTime;
            }
        }
    }


    private void CheckPlayerInRange()
    {
        if (Physics2D.OverlapCircle(transform.position, range, playerLayerMask))
        {
            playerInRange = true;
            if (player == null) player = Physics2D.OverlapCircle(transform.position, range, playerLayerMask).transform;
        }
        else
        {
            playerInRange = false;
        }
    }


    private void Shoot(Vector2 direction)
    {
        timeAfterLastShot = 0f;

        float minAngle, maxAngle;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        if (projectilesCount % 2 == 0)
        {
            minAngle = angle - spread * ((projectilesCount - 1) / 2f);
            maxAngle = angle + spread * ((projectilesCount - 1) / 2f);
        }
        else
        {
            minAngle = angle - spread * ((projectilesCount - 1) / 2);
            maxAngle = angle + spread * ((projectilesCount - 1) / 2);
        }

        minAngle *= Mathf.Deg2Rad;
        maxAngle *= Mathf.Deg2Rad;
        angle = minAngle;

        for (int i = 0; i < projectilesCount; i++)
        {
            Vector2 dir = new Vector2(-Mathf.Sin(angle), Mathf.Cos(angle));

            GameObject proj = Instantiate(projectile, transform.position, transform.rotation);
            proj.GetComponent<Rigidbody2D>().AddForce(dir * shootForce, ForceMode2D.Impulse);

            angle += spread * Mathf.Deg2Rad;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.collider.GetComponent<PlayerHealthManager>().TakeDamage(damage, transform.position.x);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay(transform.position, Vector2.up * range);
        Gizmos.DrawRay(transform.position, Vector2.left * range);
        Gizmos.DrawRay(transform.position, Vector2.right * range);
    }
}
