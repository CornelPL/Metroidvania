using UnityEngine;
using MyBox;

public class Walker_Horizontal : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int damage = 20;
    [SerializeField, MustBeAssigned] private Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] private EnemyHealthManager healthManager = null;
    [SerializeField, MustBeAssigned] private Animator animator = null;
    [Tooltip( "1 - right; -1 - left" )]
    [SerializeField] private int direction = 1;

    [Separator("Death")]

    [SerializeField, MustBeAssigned] private GameObject deadLeft = null;
    [SerializeField, MustBeAssigned] private GameObject deadRight = null;
    [SerializeField] private float deathKnockbackForce = 10f;
    [SerializeField] private float torqueOnDeath = 10f;


    public void OnDeath()
    {
        Vector3 spawnPos = new Vector3( transform.position.x, transform.position.y + 0.5f, transform.position.z );

        Rigidbody2D inst = Instantiate( direction == 1 ? deadRight : deadLeft, spawnPos, transform.rotation, null ).GetComponent<Rigidbody2D>();

        Vector2 hitDirection = -healthManager.hitDirection;
        hitDirection.y += 1f;
        hitDirection.Normalize();

        inst.AddForce( hitDirection * deathKnockbackForce, ForceMode2D.Impulse);

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
        _rigidbody.velocity = new Vector2( speed * direction, _rigidbody.velocity.y );
    }


    private void ChangeDirection()
    {
        direction = -direction;
        animator.SetBool( "isFacingRight", direction == 1 ? true : false );
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.gameObject.CompareTag( "StopMark" ) )
        {
            ChangeDirection();
        }
        else if ( collider.gameObject.CompareTag( "Player" ) )
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
}
