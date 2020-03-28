using UnityEngine;
using MyBox;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float counterattackChance = 10f;
    [SerializeField] private float counterAttackDist = 5f;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField] protected bool canBeCounterattacked = false;
    [SerializeField, MustBeAssigned] protected Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] protected GameObject destroyEffect = null;
    protected Transform player;
    protected bool canNotify = true; // so the same projectile can't be counterattacked twice
    protected bool notified = false;
    private bool checkedChances = false;


    public void SetPlayer( Transform p )
    {
        player = p;
    }


    protected virtual void Update()
    {
        if ( canBeCounterattacked && canNotify )
        {
            CheckCounterattack();
        }
    }


    protected virtual void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "DestroyableEnvironment" ) )
        {
            // TODO: Destroy it
            return;
        }

        if ( collider.gameObject.CompareTag( "Player" ) )
        {
            collider.GetComponent<PlayerHealthManager>().TakeDamage( damage, transform.position.x );
        }

        if ( notified )
        {
            player.GetComponent<Telekinesis>().NotifyCounterAttackExit( gameObject );
        }

        Instantiate( destroyEffect, transform.position, transform.rotation );
        Destroy( gameObject );
    }


    protected virtual void CheckCounterattack()
    {
        RaycastHit2D hit = Physics2D.Raycast( transform.position, _rigidbody.velocity, counterAttackDist, playerLayerMask );

        if ( !checkedChances )
        {
            if ( Random.Range( 0f, 100f ) > counterattackChance )
            {
                canNotify = false;
            }

            checkedChances = true;
        }

        if ( !notified && hit && canNotify )
        {
            player.GetComponent<Telekinesis>().NotifyCounterAttackEnter( gameObject );
            notified = true;
        }
        else if ( notified && !hit && canNotify )
        {
            canNotify = false;
            player.GetComponent<Telekinesis>().NotifyCounterAttackExit( gameObject );
        }
    }
}
