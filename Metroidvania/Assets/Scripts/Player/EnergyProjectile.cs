using System.Collections.Generic;
using UnityEngine;
using MyBox;


public class EnergyProjectile : MonoBehaviour
{
    #region Inspector variables

    [SerializeField, MustBeAssigned] protected Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] private GameObject destroyEffect = null;
    [SerializeField] private int baseDamage = 10;

    #endregion

    #region Private variables

    protected bool isShooted = false;
    protected List<Collider2D> collidersToIgnore = new List<Collider2D>();

    #endregion


    public void Shoot( Vector2 direction, float power )
    {
        gameObject.SetActive( true );
        isShooted = true;

        _rigidbody.AddForce( direction * power, ForceMode2D.Impulse );

        Collider2D[] overlapedColliders = Physics2D.OverlapCircleAll( transform.position, 0.5f );
        foreach ( Collider2D collider in overlapedColliders )
        {
            if ( !collider.CompareTag( "Enemy" ) )
            {
                collidersToIgnore.Add( collider );
            }
        }
    }


    protected virtual void OnTriggerEnter2D( Collider2D collider )
    {
        if ( !isShooted || collidersToIgnore.Find( ( Collider2D x ) => x == collider ) )
        {
            return;
        }

        if ( collider.CompareTag( "DestroyableEnvironment" ) )
        {
            // TODO: Destroy it
            return;
        }

        if ( collider.CompareTag( "Enemy" ) )
        {
            collider.GetComponent<HitManager>().TakeHit( baseDamage, _rigidbody.velocity.normalized );
        }
        else if ( collider.CompareTag( "DestroyablePlanks" ) )
        {
            collider.GetComponent<CustomDestroy>().Destroy( _rigidbody.velocity, transform.position );
        }
        else if ( collider.CompareTag( "Destroyable" ) )
        {
            collider.GetComponent<Destroyable>().GetHit();
        }

        CustomDestroy();
    }


    protected virtual void CustomDestroy()
    {
        Transform effect = Instantiate( destroyEffect, transform.position, transform.rotation ).transform;
        float angle = Mathf.Atan2( _rigidbody.velocity.y, _rigidbody.velocity.x ) * Mathf.Rad2Deg;
        effect.rotation = Quaternion.AngleAxis( angle, Vector3.forward );

        _rigidbody.velocity = Vector2.zero;
        collidersToIgnore.Clear();
        isShooted = false;
        gameObject.SetActive( false );
    }
}
