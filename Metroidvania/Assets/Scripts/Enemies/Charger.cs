using UnityEngine;
using System.Collections;
using MyBox;

public class Charger : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float chargeSpeed = 6f;
    [SerializeField] private float playerKnockbackMultiplier = 2f;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private Vector2 sightOffset = Vector2.zero;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField] private LayerMask playerAndObstaclesLayerMask = 0;
    [SerializeField, MustBeAssigned] private Animator animator = null;
    [SerializeField, MustBeAssigned] private Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] private EnemyHealthManager healthManager = null;
    [SerializeField, MustBeAssigned] private ParticleSystem breathEffect = null;
    [SerializeField, MustBeAssigned] private Vector2 breathEffectPosition = Vector2.zero;
    [SerializeField] private int direction = 1;

    private bool isCharging = false;
    private bool isStunned = false;
    private bool isWalking = false;
    private bool isPreCharging = false;
    private float timeStunned = 0f;
    private Transform player;


    public void SpawnBreathEffect()
    {
        float rand = Random.Range( 0f, 1f );
        if ( rand > 0.5f )
        {
            Vector2 pos = breathEffectPosition;
            float angle = -120f;
            if ( direction == 1 )
            {
                angle = -60f;
                pos = new Vector2( -breathEffectPosition.x, breathEffectPosition.y );
            }

            Instantiate( breathEffect, transform.position + (Vector3)pos, Quaternion.AngleAxis( angle, Vector3.forward ), null );
        }
    }


    public void OnDeath()
    {
        // TODO: spawn death parts
    }


    private void Awake()
    {
        if ( !player )
            player = GameObject.FindGameObjectWithTag( "Player" ).transform;
    }


    private void Start()
    {
        animator.SetBool( "isFacingRight", direction == 1 ? true : false );
    }


    private void Update()
    {
        if ( !isStunned && !isCharging && !isPreCharging && !healthManager.isBeingKnockbacked )
        {
            if ( CanSeePlayer() )
            {
                RotateToPlayer();
                if ( IsPlayerInRange() )
                {
                    PreCharge();
                }
            }
            else if ( !isWalking )
            {
                StartCoroutine( Walk() );
            }
        }
    }


    private bool CanSeePlayer()
    {
        Vector3 origin = (Vector2)transform.position + sightOffset;
        Vector3 vectorToPlayer = player.position - transform.position;
        return Physics2D.Raycast( origin, vectorToPlayer, Mathf.Infinity, playerAndObstaclesLayerMask ).transform.CompareTag( "Player" );
    }


    private void RotateToPlayer()
    {
        direction = player.position.x < transform.position.x ? -1 : 1;
        SetDirection( direction );
    }


    private bool IsPlayerInRange()
    {
        return Physics2D.Raycast( (Vector2)transform.position + sightOffset, new Vector2( direction, 0f ), sightRange, playerLayerMask );
    }


    private IEnumerator Walk()
    {
        animator.SetBool( "isWalking", true );

        while ( !isStunned && !isCharging && !healthManager.isBeingKnockbacked && !CanSeePlayer() )
        {
            _rigidbody.velocity = new Vector2( movementSpeed * direction, _rigidbody.velocity.y );
            yield return null;
        }

        animator.SetBool( "isWalking", false );
    }


    private void SetDirection( int dir )
    {
        direction = dir;
        animator.SetBool( "isFacingRight", direction == 1 ? true : false );
    }


    private void PreCharge()
    {
        isPreCharging = true;
        animator.SetTrigger( "preCharge" );
    }


    private void Charge()
    {
        isPreCharging = false;
        isCharging = true;
        StartCoroutine( ChargeCoroutine() );
    }


    private IEnumerator ChargeCoroutine()
    {
        while ( isCharging )
        {
            _rigidbody.velocity = new Vector2( chargeSpeed * direction, _rigidbody.velocity.y );

            yield return null;
        }
    }


    private IEnumerator Stun()
    {
        isStunned = true;
        animator.SetBool( "isStunned", true );

        while( isStunned && timeStunned < stunTime )
        {
            timeStunned += Time.deltaTime;
            yield return null;
        }

        timeStunned = 0f;
        animator.SetBool( "isStunned", false );
        isStunned = false;
        SetDirection( -direction );
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( isCharging )
        {
            isCharging = false;
            StartCoroutine( Stun() );
        }
        else if ( collision.collider.CompareTag( "Wall" ) )
        {
            SetDirection( -direction );
        }
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( !isCharging && collider.gameObject.CompareTag( "StopMark" ) )
        {
            SetDirection( -direction );
        }
        else if ( collider.CompareTag( "Player" ) )
        {
            float knockbackMultiplier = isCharging ? playerKnockbackMultiplier : 1f;
            collider.GetComponent<PlayerHealthManager>().TakeDamage( damage, transform.position.x, knockbackMultiplier );
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay( (Vector2)transform.position + sightOffset, new Vector2(direction * sightRange, 0f));
    }
}
