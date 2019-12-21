using UnityEngine;
using MyBox;

public class ShootingPlantProjectile : EnemyProjectile
{
    [SerializeField] private float shootForce = 100f;
    [SerializeField, MustBeAssigned] private Rigidbody2D _rigidbody = null;

    private bool isShot = false;
    private Transform player;


    public void SetPlayer( Transform p )
    {
        player = p;
    }


    private void Update()
    {
        if ( !isShot && _rigidbody.velocity.y <= 0f )
        {
            Vector2 direction = player.position + Vector3.up * 2f - transform.position;
            _rigidbody.AddForce( direction.normalized * shootForce, ForceMode2D.Impulse );
            _rigidbody.gravityScale = 0f;
            // TODO: Shoot effects

            isShot = true;
        }
    }
}
