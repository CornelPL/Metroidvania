using UnityEngine;
using MyBox;

public class ShootingPlantProjectile : EnemyProjectile
{
    [SerializeField] private float shootForce = 100f;
    [SerializeField, MustBeAssigned] private GameObject shootEffect = null;

    private bool isShot = false;


    protected override void Update()
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
        else if ( isShot && canNotify && canBeCounterattacked )
        {
            CheckCounterattack();
        }
    }
}
