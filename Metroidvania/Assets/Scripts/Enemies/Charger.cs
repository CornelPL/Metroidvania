using UnityEngine;

public class Charger : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float sightRange = 5f;
    [SerializeField] private float chargeSpeed = 6f;
    [SerializeField] private float playerKnockbackMultiplier = 2f;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private Vector2 sightOffset = Vector2.zero;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private EnemyHealthManager healthManager = null;

    private int direction = 1;
    private bool isCharging = false;
    private bool isStunned = false;
    private float timeStunned = 0f;
    private Transform player;


    private void Awake()
    {
        if ( !player )
        {
            player = GameObject.FindGameObjectWithTag( "Player" ).transform;
        }
    }


    private void Update()
    {
        if (!isStunned)
        {
            if (!healthManager.isBeingKnockbacked && !isCharging)
            {
                RotateToPlayer();
                CheckPlayerInSight();
            }
            else if (isCharging)
            {
                Charge();
            }
        }
        else if (timeStunned < stunTime)
        {
            // TODO: Play stunned anim
            timeStunned += Time.deltaTime;
        }
        else
        {
            timeStunned = 0f;
            isStunned = false;
        }
    }


    private void RotateToPlayer()
    {
        // TODO: Rotate sprite
        direction = player.position.x < transform.position.x ? -1 : 1;
    }


    private void CheckPlayerInSight()
    {
        if ( Physics2D.Raycast( (Vector2)transform.position + sightOffset, new Vector2( direction, 0f ), sightRange, playerLayerMask ) )
        {
            isCharging = true;
            Charge();
        }
    }


    private void Charge()
    {
        _rigidbody.velocity = new Vector2(chargeSpeed * direction, _rigidbody.velocity.y);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( isCharging )
        {
            isCharging = false;
            isStunned = true;
        }
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Player" ) )
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
