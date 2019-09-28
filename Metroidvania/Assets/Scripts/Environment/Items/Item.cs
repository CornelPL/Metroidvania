using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;


public enum ItemType
{
    rock,
    crate,
    plank,
    barrel,
    saw
}


public class Item : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private Collider2D _collider = null;
    [SerializeField] private ItemType itemType = ItemType.rock;
    [SerializeField] private List<GameObject> itemsToSpawn = null;
    [SerializeField] private int minItemsToSpawn = 1;
    [SerializeField] private int maxItemsToSpawn = 3;
    [SerializeField] private int plankHealth = 3;
    [SerializeField] private GameObject destroyEffect = null;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private float knockbackForce = 100f;

    // Add PullParticles.Play();
    [SerializeField] private UnityEvent OnStartPulling = null;
    // Add HoldingParticles.Play();
    [SerializeField] private UnityEvent OnPullingCompleted = null;
    // Add HoldingParticles.Stop();
    [SerializeField] private UnityEvent OnRelease = null;

    private Transform itemHolder = null;
    private bool isPulling = false;
    private float pullSpeedUp = 0f;
    private float maxPullSpeed = 0f;
    private float pullingTime = 1f;
    private float gravityScaleCopy = 0f;
    private List<Collider2D> collidersToIgnore = new List<Collider2D>();


    public void SetFree()
    {
        transform.SetParent( null );
        _rigidbody.simulated = true;
        OnRelease.Invoke();
    }


    public void StartPulling( Transform t, float s, float ms )
    {
        itemHolder = t;
        pullSpeedUp = s;
        maxPullSpeed = ms;

        isPulling = true;
        PlayerState.instance.isPullingItemState = true;

        OnStartPulling.Invoke();

        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.gravityScale = 0f;

        _collider.enabled = false;

        if ( itemType == ItemType.plank )
            gameObject.layer = LayerMask.NameToLayer( "Planks" );
    }


    public void AbortPulling()
    {
        PlayerState.instance.isPullingItemState = false;
        _rigidbody.gravityScale = gravityScaleCopy;
        _collider.enabled = true;
        gameObject.layer = LayerMask.NameToLayer( "Items" );
        this.enabled = false;
    }


    public void Shoot( Vector2 direction, float power )
    {
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


    private void Awake()
    {
        gravityScaleCopy = _rigidbody.gravityScale;

        Assert.IsNotNull( _rigidbody );
        Assert.IsNotNull( _collider );
    }


    private void Update()
    {
        if ( isPulling )
        {
            Pull();
        }
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        // DELETED "if (isShooted)"
        if ( collidersToIgnore.Find( ( Collider2D x ) => x == collider ) )
        {
            return;
        }

        GameObject go = collider.gameObject;
        if ( go.CompareTag( "Enemy" ) )
        {
            Vector2 direction = GetComponent<Rigidbody2D>().velocity.normalized;

            go.GetComponent<HitManager>().TakeHit( baseDamage, direction, knockbackForce );
            CustomDestroy();
        }
        else if ( go.CompareTag( "DestroyablePlanks" ) )
        {
            go.GetComponent<CustomDestroy>().Destroy();
        }

        if ( itemType == ItemType.rock )
        {
            CustomDestroy();
        }
        else if ( itemType == ItemType.crate )
        {
            CustomDestroy();
        }
        else if ( itemType == ItemType.plank )
        {
            if ( go.CompareTag( "SoftWall" ) )
            {
                plankHealth--;
                if ( plankHealth == 0 ) CustomDestroy();
                _rigidbody.velocity = Vector2.zero;
                _rigidbody.bodyType = RigidbodyType2D.Static;
                gameObject.layer = LayerMask.NameToLayer( "PlanksGround" );
                transform.rotation = Quaternion.identity;
            }
            else if ( !go.CompareTag( "Player" ) )
            {
                CustomDestroy();
            }
        }
    }


    private void Pull()
    {
        pullingTime += Time.deltaTime;

        UpdateVelocity();

        if ( IsItemOnPlace() )
        {
            FinishPulling();
        }
    }


    private void UpdateVelocity()
    {
        Vector2 direction = itemHolder.position - transform.position;
        direction.Normalize();

        float speed = _rigidbody.velocity.magnitude;

        if ( speed < maxPullSpeed )
        {
            speed += pullSpeedUp * pullingTime;
        }

        _rigidbody.velocity = direction * speed;
    }


    private bool IsItemOnPlace()
    {
        float distance = Vector2.Distance( transform.position, itemHolder.position );

        return distance < 1f;
    }


    private void FinishPulling()
    {
        PlayerState.instance.isHoldingItemState = true;

        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        _rigidbody.simulated = false;

        transform.SetParent( itemHolder );
        transform.position = itemHolder.position;

        pullingTime = 1f;

        OnPullingCompleted.Invoke();

        AbortPulling();
    }


    private void CustomDestroy()
    {
        Vector2 collisionVelocity = _rigidbody.velocity;

        if ( itemType == ItemType.crate )
        {
            int item = Random.Range( 0, itemsToSpawn.Count );
            int i = Random.Range( minItemsToSpawn, maxItemsToSpawn );

            for ( int a = 0; a < i; a++ )
            {
                Vector3 randomRotation = transform.eulerAngles;
                randomRotation.z = Random.Range( 0f, 360f );
                GameObject inst = Instantiate( itemsToSpawn[ item ], transform.position + (Vector3)Random.insideUnitCircle, transform.rotation );
                inst.transform.eulerAngles = randomRotation;
                inst.GetComponent<Rigidbody2D>().velocity = -collisionVelocity;
            }
        }

        Transform effect = Instantiate( destroyEffect, transform.position, transform.rotation ).transform;
        float angle = Mathf.Atan2( _rigidbody.velocity.y, _rigidbody.velocity.x ) * Mathf.Rad2Deg;
        effect.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
        Destroy( gameObject );
    }
}
