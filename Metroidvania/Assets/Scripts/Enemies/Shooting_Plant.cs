using UnityEngine;
using MyBox;

public class Shooting_Plant : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float range = 5f;
    [SerializeField] private float timeBetweenShots = 1f;
    [SerializeField] private float shootForce = 15f;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField, MustBeAssigned] private Transform shootPosition = null;
    [SerializeField, MustBeAssigned] private GameObject projectile = null;
    [SerializeField, MustBeAssigned] private Animator animator = null;
    [SerializeField, MustBeAssigned] private ExplosionLight _light = null;
    [SerializeField, MustBeAssigned] private ExplosionLight upperLight = null;
    [SerializeField, MustBeAssigned] private ParticleSystem chargeParticles = null;
    [SerializeField, MustBeAssigned] private ParticleSystem shootParticles = null;

    private bool isCharging = false;
    private float lastTimeShot = 0f;
    private Transform player = null;


    private void Update()
    {
        if ( !isCharging && IsPlayerInRange() && Time.time - lastTimeShot > timeBetweenShots )
        {
            ChargeShoot();
        }
    }


    private bool IsPlayerInRange()
    {
        Collider2D col = Physics2D.OverlapCircle( transform.position, range, playerLayerMask );
        if ( col != null )
        {
            if (player == null) player = col.transform;
            return true;
        }

        return false;
    }


    private void ChargeShoot()
    {
        isCharging = true;
        animator.SetTrigger( "charge" );
        _light.FadeIn( true );
        upperLight.FadeIn( true );
        if ( !chargeParticles.isPlaying )
        {
            chargeParticles.Play();
        }
    }


    private void Shoot()
    {
        isCharging = false;

        GameObject proj = Instantiate( projectile, shootPosition.position, transform.rotation );
        proj.GetComponent<Rigidbody2D>().AddForce( Vector2.up * shootForce, ForceMode2D.Impulse );
        proj.GetComponent<ShootingPlantProjectile>().SetPlayer( player );

        if ( !shootParticles.isPlaying )
        {
            shootParticles.Play();
        }

        lastTimeShot = Time.time;
    }


    /*private void Shoot(Vector2 direction)
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
    }*/


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
