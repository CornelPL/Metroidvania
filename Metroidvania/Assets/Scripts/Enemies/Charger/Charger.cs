using UnityEngine;
using MyBox;
using System.Collections.Generic;

public class Charger : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private float playerKnockbackMultiplier = 2f;
    [SerializeField] private float breathEffectSpawnChance = 30f;
    [SerializeField] private Vector2 sightOffset = Vector2.zero;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField] private LayerMask playerAndObstaclesLayerMask = 0;
    [SerializeField, MustBeAssigned] private Animator animator = null;
    [SerializeField, MustBeAssigned] private GameObject hitWallEffect = null;
    [SerializeField] private Vector2 hitWallEffectPos = Vector2.zero;
    [SerializeField, MustBeAssigned] private ParticleSystem breathEffect = null;
    [SerializeField, MustBeAssigned] private Vector2 breathEffectPosition = Vector2.zero;
    [SerializeField, MustBeAssigned] private List<GameObject> deathParts = null;
    [SerializeField] private float deathPartsForce = 10f;
    [SerializeField] private float torqueOnDeath = 1f;

    public int direction = 1;
    [HideInInspector] public bool isCharging = false;

    private Transform player;


    public void SpawnBreathEffect()
    {
        float rand = Random.Range( 0f, 100f );
        if ( rand < breathEffectSpawnChance )
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
        Vector2 spawnPos = new Vector2( transform.position.x, transform.position.y + 0.5f );

        for ( int i = 0; i < deathParts.Count; i++ )
        {
            Rigidbody2D inst = Instantiate( deathParts[ i ], spawnPos, Quaternion.identity, null ).GetComponent<Rigidbody2D>();

            Vector2 dir = Random.insideUnitCircle;
            dir.y = Mathf.Abs( dir.y );

            inst.AddForce( dir * deathPartsForce, ForceMode2D.Impulse );

            inst.AddTorque( torqueOnDeath, ForceMode2D.Impulse );

        }

        gameObject.SetActive( false );
    }


    private void Awake()
    {
        if ( !player )
            player = GameObject.FindGameObjectWithTag( "Player" ).transform;
    }


    private void OnEnable()
    {
        animator.SetBool( "isFacingRight", direction == 1 ? true : false );
    }


    public bool CanSeePlayer()
    {
        Vector3 origin = (Vector2)transform.position + sightOffset;
        Vector3 vectorToPlayer = player.position - transform.position;
        RaycastHit2D raycastHit = Physics2D.Raycast( origin, vectorToPlayer, Mathf.Infinity, playerAndObstaclesLayerMask );
        if ( raycastHit )
        {
            return raycastHit.transform.CompareTag( "Player" );
        }

        return false;
    }


    public bool IsPlayerInRange()
    {
        return Physics2D.Raycast( (Vector2)transform.position + sightOffset, new Vector2( direction, 0f ), sightRange, playerLayerMask );
    }


    public int SetDirection( int dir = 0 )
    {
        if ( dir == 0 )
        {
            direction = player.position.x < transform.position.x ? -1 : 1;
        }
        else
        {
            direction = dir;
        }

        animator.SetBool( "isFacingRight", direction == 1 ? true : false );

        return direction;
    }


    private void OnCollisionEnter2D( Collision2D collision )
    {
        float contactX = collision.GetContact( 0 ).normal.x;
        if ( contactX < -0.5f || contactX > 0.5f )
        {
            if ( isCharging )
            {
                animator.SetBool( "isStunned", true );

                float angle = direction == 1 ? 180f : 0f;
                Vector2 pos = new Vector2( hitWallEffectPos.x * direction, hitWallEffectPos.y );
                Instantiate( hitWallEffect, (Vector2)transform.position + pos, Quaternion.AngleAxis( angle, Vector3.forward ), null );
            }
            else
            {
                SetDirection( -direction );
            }
        }
        else if ( collision.collider.CompareTag( "Spikes" ) )
        {
            OnDeath();
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
        Gizmos.DrawRay( (Vector2)transform.position + sightOffset, new Vector2( direction * sightRange, 0f ) );
    }
}
