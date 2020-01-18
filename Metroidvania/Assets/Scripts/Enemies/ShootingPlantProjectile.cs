using UnityEngine;
using MyBox;

public class ShootingPlantProjectile : EnemyProjectile
{
    [SerializeField] private float shootForce = 100f;
    [SerializeField] private float counterAttackDist = 5f;
    [SerializeField] private LayerMask playerLayerMask = 0;
    [SerializeField, MustBeAssigned] private Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] private GameObject shootEffect = null;

    private bool isShot = false;
    private bool notified = false;
    private bool canNotify = true; // so the same projectile can't be counterattacked twice
    private Transform player;


    public void SetPlayer( Transform p )
    {
        player = p;
    }


    private void Update()
    {

        if ( !isShot && _rigidbody.velocity.y <= 0.1f )
        {
            Vector2 direction = player.position + Vector3.up * 1f - transform.position;
            _rigidbody.AddForce( direction.normalized * shootForce, ForceMode2D.Impulse );
            _rigidbody.gravityScale = 0f;

            float angle = Mathf.Atan2( direction.y, direction.x ) * Mathf.Rad2Deg;

            Instantiate( shootEffect, transform.position, Quaternion.AngleAxis( angle, Vector3.forward ) );

            isShot = true;
        }
        else if ( isShot && canNotify )
        {
            RaycastHit2D hit = Physics2D.Raycast( transform.position, _rigidbody.velocity, counterAttackDist, playerLayerMask );

            if ( !notified && hit )
            {
                notified = true;
                player.GetComponent<Telekinesis>().NotifyCounterAttackEnter( gameObject );
            }
            else if ( notified && !hit )
            {
                canNotify = false;
                player.GetComponent<Telekinesis>().NotifyCounterAttackExit( gameObject );
            }
        }
    }


    protected override void OnCollisionEnter2D( Collision2D collision )
    {
        base.OnCollisionEnter2D( collision );

        player.GetComponent<Telekinesis>().NotifyCounterAttackExit( gameObject );
    }
}
