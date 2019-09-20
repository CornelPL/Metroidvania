using UnityEngine;

public class Walker_Horizontal : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minSpeed = 0.5f;
    [SerializeField] private int damage = 20;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private EnemyHealthManager healthManager = null;
    [SerializeField] private Animator animator = null;
    [Tooltip("1 - right; -1 - left")]
    [SerializeField] private int direction = 1;
    [SerializeField] private float torqueOnDeath = 10f;
    [SerializeField] private GameObject onDeathColliderLeft = null;
    [SerializeField] private GameObject onDeathColliderRight = null;

    private float timeWalkingTooSlow = 0f;


    public void Death()
    {
        animator.SetBool( "isDead", true );

        if ( direction == 1 )
        {
            onDeathColliderRight.SetActive( true );
        }
        else
        {
            onDeathColliderLeft.SetActive( true );
        }

        _rigidbody.freezeRotation = false;
        _rigidbody.AddTorque( torqueOnDeath * direction, ForceMode2D.Impulse );

        healthManager.enabled = false;
        this.enabled = false;
    }


    private void Start()
    {
        animator.SetBool( "isFacingRight", direction == 1 ? true : false );
    }


    private void Update()
    {
        if (!healthManager.isBeingKnockbacked)
        {
            Move();
        }
        else if ( _rigidbody.velocity.magnitude < minSpeed )
        {
            healthManager.isBeingKnockbacked = false;
        }
    }

    private void Move()
    {
        // TODO: Change moving detection in all walkers to collision, not actual speed
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


    private void ChangeDirection()
    {
        direction = direction > 0 ? -1 : 1;
        animator.SetBool( "isFacingRight", direction == 1 ? true : false );
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if ( collider.gameObject.CompareTag("StopMark"))
        {
            ChangeDirection();
        }
        else if ( collider.gameObject.CompareTag( "Player" ) )
        {
            collider.GetComponent<PlayerHealthManager>().TakeDamage( damage, transform.position.x );
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (healthManager.isBeingKnockbacked)
        {
            healthManager.isBeingKnockbacked = false;
        }
    }
}
