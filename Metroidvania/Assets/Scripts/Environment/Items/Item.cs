using System.Collections.Generic;
using UnityEngine;
using MyBox;


public class Item : MonoBehaviour
{
    #region Inspector variables

    [SerializeField, MustBeAssigned] protected Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] private Collider2D _collider = null;
    [SerializeField, MustBeAssigned] private GameObject destroyEffect = null;
    [SerializeField] private GameObject itemHighlight = null;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float knockbackForce = 100f;

    #endregion

    [HideInInspector] public bool isSpawned = false;

    #region Private variables

    private Transform itemHolder = null;
    private bool isPulling = false;
    protected bool isShooted = false;
    private float pullSpeedUp = 0f;
    private float maxPullSpeed = 0f;
    private float pullingTime = 1f;
    private float gravityScaleCopy = 0f;
    private float previousDistance = 0f;
    private Vector2 calculatedVelocity = Vector2.zero;
    protected List<Collider2D> collidersToIgnore = new List<Collider2D>();

    #endregion

    public virtual void MakeItem()
    {
        GetComponent<Collider2D>().isTrigger = true;
        _rigidbody.velocity = Vector2.zero;
    }


    public virtual void SetFree()
    {
        transform.SetParent( null );
        _rigidbody.simulated = true;
    }


    public virtual void StartPulling( Transform t, float s, float ms )
    {
        itemHolder = t;
        pullSpeedUp = s;
        maxPullSpeed = ms;

        isPulling = true;

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.gravityScale = 0f;

        _collider.enabled = false;
    }


    public virtual void AbortPulling()
    {
        PlayerState.instance.isPullingItemState = false;
        _rigidbody.gravityScale = gravityScaleCopy;
        _collider.enabled = true;
        gameObject.layer = LayerMask.NameToLayer( "Items" );
        isPulling = false;
    }


    public virtual void Shoot( Vector2 direction, float power )
    {
        isShooted = true;

        _rigidbody.AddForce( direction * power, ForceMode2D.Impulse );

        _collider.enabled = true;
        _collider.isTrigger = true;

        gameObject.layer = LayerMask.NameToLayer( "ShootItem" );

        Collider2D[] overlapedColliders = Physics2D.OverlapCircleAll( transform.position, 0.5f );
        foreach ( Collider2D collider in overlapedColliders )
        {
            if ( !collider.CompareTag( "Enemy" ) )
            {
                collidersToIgnore.Add( collider );
            }
        }
    }


    public virtual void OnHover( bool start )
    {
        itemHighlight?.SetActive( start );
    }


    protected virtual void Awake()
    {
        gravityScaleCopy = _rigidbody.gravityScale;
    }


    protected virtual void FixedUpdate()
    {
        if ( isPulling )
        {
            Pull();
        }
    }


    protected virtual void OnTriggerEnter2D( Collider2D collider )
    {
        GameObject go = collider.gameObject;

        if ( go.CompareTag( "Enemy" ) )
        {
            go.GetComponent<HitManager>().TakeHit( baseDamage, _rigidbody.velocity.normalized, knockbackForce );
        }
        else if ( go.CompareTag( "DestroyablePlanks" ) )
        {
            go.GetComponent<CustomDestroy>().Destroy( _rigidbody.velocity, transform.position );
        }
        else if ( go.CompareTag( "Destroyable" ) )
        {
            go.GetComponent<Destroyable>().GetHit();
        }
    }


    protected virtual void Pull()
    {
        pullingTime += Time.deltaTime;

        CalculateVelocity();

        if ( IsItemOnPlace() )
        {
            FinishPulling();
        }
        else
        {
            UpdateVelocity();
        }
    }


    protected virtual void CalculateVelocity()
    {
        Vector2 direction = itemHolder.position - transform.position;
        direction.Normalize();

        float speed = _rigidbody.velocity.magnitude;

        if ( speed < maxPullSpeed )
        {
            speed += pullSpeedUp * pullingTime * pullingTime;
            if ( speed > maxPullSpeed )
                speed = maxPullSpeed;
        }

        calculatedVelocity = direction * speed;
    }


    protected virtual void UpdateVelocity()
    {
        _rigidbody.velocity = calculatedVelocity;
    }


    protected virtual bool IsItemOnPlace()
    {
        float currentDistance = Vector2.Distance( transform.position, itemHolder.position );
        float nextDistance = Vector2.Distance( (Vector2)transform.position + calculatedVelocity * Time.fixedDeltaTime, itemHolder.position );

        if ( nextDistance - currentDistance > previousDistance )
        {
            previousDistance = 0;
            return true;
        }
        else
        {
            previousDistance = nextDistance - currentDistance;
            return false;
        }
    }


    protected virtual void FinishPulling()
    {
        PlayerState.instance.isHoldingItemState = true;

        itemHolder.GetComponent<HoldingItemPlacePosition>().Knockback( _rigidbody.velocity.normalized );

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.simulated = false;

        transform.SetParent( itemHolder );
        transform.position = itemHolder.position;

        pullingTime = 1f;

        AbortPulling();
    }


    protected virtual void CustomDestroy()
    {
        Transform effect = Instantiate( destroyEffect, transform.position, transform.rotation ).transform;
        float angle = Mathf.Atan2( _rigidbody.velocity.y, _rigidbody.velocity.x ) * Mathf.Rad2Deg;
        effect.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
        Destroy( gameObject );
    }
}
