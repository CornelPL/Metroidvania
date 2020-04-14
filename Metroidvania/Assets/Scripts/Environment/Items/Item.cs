using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.Events;

public class Item : MonoBehaviour
{
    #region Inspector variables

    [SerializeField, MustBeAssigned] protected Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] protected Collider2D _collider = null;
    [SerializeField, MustBeAssigned] private GameObject destroyEffect = null;
    [SerializeField] private GameObject itemHighlight = null;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float knockbackForce = 100f;
    [SerializeField] protected UnityEvent onTriggerEnterHitEvent = null;

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
    private Telekinesis telekinesis = null;

    #endregion


    /// <summary>
    /// A test class
    /// </summary>
    // <param name="t">Used to indicate status.</param>
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


    public virtual void StartPulling( Transform t, float s, float ms, Telekinesis tele )
    {
        itemHolder = t;
        pullSpeedUp = s;
        maxPullSpeed = ms;
        telekinesis = tele;

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
    }


    public virtual void OnHover( bool start )
    {
        if ( itemHighlight != null )
            itemHighlight.SetActive( start );
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
        if ( OnTriggerEnter2DHit( collider ) )
            onTriggerEnterHitEvent.Invoke();
    }


    protected bool OnTriggerEnter2DHit( Collider2D collider )
    {
        // TODO: is it needed?
        if ( !isShooted )
            return false;

        string tmpTag = collider.tag;
        switch ( tmpTag )
        {
            case "DestroyableEnvironment":
                // TODO: Destroy this enviro
                return false;
            case "Enemy":
                collider.GetComponent<HitManager>().TakeHit( baseDamage, _rigidbody.velocity.normalized, knockbackForce );
                return true;
            case "DestroyablePlanks":
                collider.GetComponent<DestroyablePlanks>().GetHit( baseDamage, _rigidbody.velocity );
                return true;
            case "Destroyable":
                collider.GetComponent<Destroyable>().GetHit();
                return true;
            default:
                return true;
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


    public virtual void CustomDestroy()
    {
        Instantiate( destroyEffect, transform.position, transform.rotation );
        //Transform effect = Instantiate( destroyEffect, transform.position, transform.rotation ).transform;
        //float angle = Mathf.Atan2( _rigidbody.velocity.y, _rigidbody.velocity.x ) * Mathf.Rad2Deg;
        //effect.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
        Destroy( gameObject );
    }
}
