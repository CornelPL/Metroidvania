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
    [SerializeField, MustBeAssigned] private ParticleSystem[] shootParticles = null;

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

        for ( int i = 0; i < shootParticles.Length; i++ )
        {
            if ( !shootParticles[ i ].isPlaying )
            {
                shootParticles[ i ].Play();
            }
        }

        lastTimeShot = Time.time;
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
        Gizmos.DrawRay( transform.position, Vector2.up * range );
        Gizmos.DrawRay( transform.position, Vector2.left * range );
        Gizmos.DrawRay( transform.position, Vector2.right * range );

        float projectileHeight = 8f * Mathf.Pow( shootForce / projectile.GetComponent<Rigidbody2D>().mass, 2 ) / 25f / -Physics2D.gravity.y;

        Gizmos.color = Color.red;
        Gizmos.DrawRay( shootPosition.position, Vector2.up * projectileHeight );
    }
}
