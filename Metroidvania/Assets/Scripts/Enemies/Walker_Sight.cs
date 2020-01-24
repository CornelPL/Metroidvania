using UnityEngine;
using MyBox;

public class Walker_Sight : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int damage = 20;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private Vector2 sightOffset = Vector2.zero;
    [SerializeField] private float boost = 1.5f;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private EnemyHealthManager healthManager = null;
    [SerializeField] private Animator animator = null;
    [Tooltip( "1 - right; -1 - left" )]
    [SerializeField] private int direction = 1;

    [Separator( "Death" )]

    [SerializeField, MustBeAssigned] private GameObject deadLeft = null;
    [SerializeField, MustBeAssigned] private GameObject deadRight = null;
    [SerializeField] private float deathKnockbackForce = 10f;
    [SerializeField] private float torqueOnDeath = 1f;

    private bool isPlayerInSight = false;


    public void OnDeath()
    {
        Vector3 spawnPos = new Vector3( transform.position.x, transform.position.y + 0.5f, transform.position.z );

        Rigidbody2D inst = Instantiate( direction == 1 ? deadRight : deadLeft, spawnPos, transform.rotation, null ).GetComponent<Rigidbody2D>();

        Vector2 hitDirection = healthManager.hitDirection;
        hitDirection.y += 1f;
        hitDirection.Normalize();

        inst.AddForce( hitDirection * deathKnockbackForce, ForceMode2D.Impulse );

        float moveDirection = Mathf.Sign( _rigidbody.velocity.x );
        inst.AddTorque( torqueOnDeath * moveDirection, ForceMode2D.Impulse );

        Destroy( gameObject );
    }


    private void Start()
    {
        animator.SetBool( "isFacingRight", direction == 1 ? true : false );
    }


    private void Update()
    {
        if ( !healthManager.isBeingKnockbacked )
        {
            Move();
        }
    }


    private void Move()
    {
        if ( IsPlayerInSight() != isPlayerInSight )
        {
            isPlayerInSight = !isPlayerInSight;
            animator.SetBool( "isAttacking", isPlayerInSight );
        }

        float s = isPlayerInSight ? boost : 1f;
        s = s * speed * direction;
        _rigidbody.velocity = new Vector2( s, _rigidbody.velocity.y );
    }


    private bool IsPlayerInSight()
    {
        RaycastHit2D hitr = Physics2D.Raycast( (Vector2)transform.position + sightOffset, new Vector2( direction, 0f ), sightRange, playerLayerMask );
        RaycastHit2D hitl = Physics2D.Raycast( (Vector2)transform.position + sightOffset, new Vector2( -direction, 0f ), sightRange, playerLayerMask );

        if ( hitr && !hitr.transform.CompareTag( "StopMark" ) )
        {
            return true;
        }
        else if ( hitl && !hitl.transform.CompareTag( "StopMark" ) )
        {
            ChangeDirection();
            return true;
        }

        return false;
    }


    private void ChangeDirection()
    {
        direction = direction > 0 ? -1 : 1;
        animator.SetBool( "isFacingRight", direction == 1 ? true : false );
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "StopMark" ) )
        {
            ChangeDirection();
        }
        else if ( collider.CompareTag( "Player" ) )
        {
            collider.GetComponent<PlayerHealthManager>().TakeDamage( damage, transform.position.x );
        }
    }


    private void OnCollisionEnter2D( Collision2D collision )
    {
        if ( collision.collider.CompareTag( "Wall" ) )
        {
            ChangeDirection();
        }

        if ( healthManager.isBeingKnockbacked )
        {
            healthManager.isBeingKnockbacked = false;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawRay( (Vector2)transform.position + sightOffset, new Vector2( sightRange, 0f ) );
        Gizmos.DrawRay( (Vector2)transform.position + sightOffset, new Vector2( -sightRange, 0f ) );
    }
}