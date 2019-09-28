using UnityEngine;

public class Walker_Horizontal : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int damage = 20;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private EnemyHealthManager healthManager = null;
    [SerializeField] private Animator animator = null;
    [Tooltip( "1 - right; -1 - left" )]
    [SerializeField] private int direction = 1;


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
        direction = direction > 0 ? -1 : 1;
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
