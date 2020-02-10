using UnityEngine;
using Cinemachine;
using MyBox;

public class DemoBoss : MonoBehaviour
{
    [Separator( "General" )]
    [SerializeField] private int secondPhaseHP = 10;
    [SerializeField] private int thirdPhaseHP = 5;
    [SerializeField] private int touchDamage = 1;
    [SerializeField] private BossHealthManager healthManager = null;
    [SerializeField] private Animator _animator = null;
    [SerializeField] private DemoBossRoom room = null;

    [Separator( "Shooting" )]
    [SerializeField] private Transform shootPosition = null;
    [SerializeField] private Transform projectile = null;
    [SerializeField] private float forceVariation = 0.2f;
    [SerializeField] private float angleVariation = 0.2f;

    [Separator( "Armoring" )]
    public ParticleSystem[] armoringParticles = null;
    public GameObject forceField = null;

    [Separator( "Charge" )]
    [SerializeField] private int chargeDamage = 2;
    [SerializeField] private float chargeKnockbackMultiplier = 2f;
    [SerializeField] private Vector2Event EarthquakeEvent = null;

    [Separator( "Rage" )]
    public ParticleSystem[] rageParticles = null;
    public CinemachineImpulseSource OnRageImpulse = null;

    [Separator( "Effects" )]
    [SerializeField] private GameObject hitWallEffects = null;
    [SerializeField] private Vector2 hitWallEffectsPos = Vector2.zero;

    public bool wasShooting = false;
    public Transform player;
    public int direction = 1;

    [HideInInspector] public int currentSequence = 0;
    [HideInInspector] public bool isCharging = false;
    [HideInInspector] public bool isMoving = false;
    private int phase = 1;


    public void SetDirection()
    {
        direction = player.position.x < transform.position.x ? -1 : 1;
        _animator.SetInteger( "direction", direction );
    }


    public void AssignPlayer( Transform p )
    {
        player = p;
    }


    public void HitInWeakPoint()
    {
        if ( phase == 1 || phase == 2 )
        {
            _animator.SetBool( "isStunned", false );
            _animator.SetBool( "isArmored", false );
        }
        else
        {
            _animator.Play( "Demo_boss_death" );
        }
    }


    public void CheckPhaseHP()
    {
        if ( (phase == 1 && healthManager.currentHP <= secondPhaseHP) ||
             (phase == 2 && healthManager.currentHP <= thirdPhaseHP) ||
             (phase == 3 && healthManager.currentHP <= 0) )
        {
            _animator.Play( "Demo_boss_armor_up" );
        }
    }


    public void ShootProjectile()
    {
        Rigidbody2D rb = Instantiate( projectile, shootPosition.position, transform.rotation ).GetComponent<Rigidbody2D>();

        Vector2 vectorToPlayer = player.position - shootPosition.position;

        float angleToPlayer = Mathf.Atan2( vectorToPlayer.y, vectorToPlayer.x ) * Mathf.Rad2Deg;
        if ( angleToPlayer < -90f )
            angleToPlayer = -(180f + angleToPlayer);
        else if ( angleToPlayer > 90f )
        {
            angleToPlayer = 180f - angleToPlayer;
        }
        float angle = 45;
        if ( angleToPlayer < 0f )
        {
            angle -= angleToPlayer / 2f;
            vectorToPlayer.y = 0f;
        }
        else
        {
            angle += angleToPlayer / 2f;
        }
        angle = Random.Range( angle * (1f - angleVariation), angle * (1f + angleVariation) );
        if ( angle > 89f ) angle = 89f;
        angle *= Mathf.Deg2Rad;

        Vector2 dir = new Vector2( Mathf.Cos( angle ) * direction, Mathf.Sin( angle ) );
        float distance = vectorToPlayer.magnitude;
        float force = Mathf.Sqrt( -Physics2D.gravity.y * rb.gravityScale * distance / Mathf.Sin( 2f * angle ) );
        force = Random.Range( force * (1f - forceVariation), force * (1f + forceVariation) );
        rb.AddForce( dir * force, ForceMode2D.Impulse );
    }


    private void StopCharging()
    {
        _animator.SetBool( "isStunned", true );

        EarthquakeEvent.Broadcast( gameObject, transform.position );

        float angle = direction == 1 ? 180f : 0f;
        Vector2 pos = new Vector2( hitWallEffectsPos.x * direction, hitWallEffectsPos.y );
        Instantiate( hitWallEffects, (Vector2)transform.position + pos, Quaternion.AngleAxis( angle, Vector3.forward ), null );

        room.Earthquake();
    }


    private void OnCollisionEnter2D( Collision2D collision )
    {
        if ( collision.collider.CompareTag( "Wall" ) )
        {
            if ( isMoving )
            {
                _animator.SetBool( "isMoving", false );
            }
            else if ( isCharging )
            {
                StopCharging();
            }
        }
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Player" ) )
        {
            int damage = touchDamage;
            float knockbackMultiplier = 1f;

            if ( isCharging )
            {
                damage = chargeDamage;
                knockbackMultiplier = chargeKnockbackMultiplier;
            }

            collider.GetComponent<PlayerHealthManager>().TakeDamage( damage, transform.position.x, knockbackMultiplier );
        }
    }
}
