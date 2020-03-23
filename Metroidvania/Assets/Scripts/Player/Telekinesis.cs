using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class Telekinesis : MonoBehaviour
{
    #region Editor variables

    [SerializeField] private float range = 10f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float pullSpeed = 50f;
    [SerializeField] private float maxPullSpeed = 50f;
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private float slowmoMaxTime = 2f;
    [SerializeField] private float leanWeight = 0.1f;
    [SerializeField] private float leanTime = 0.5f;
    [SerializeField] private float zeroOffsetWeight = 0.5f;
    [SerializeField] private float invulnerableTime = 0.5f;
    [SerializeField] public int pullFromSurfaceCost = 10;
    [SerializeField, MustBeAssigned] private CinemachineCameraOffset cameraOffset = null;
    [SerializeField, MustBeAssigned] private Transform holdingItemPlace = null;
    [SerializeField, MustBeAssigned] private GameObject[] rocksToSpawn = null;
    [SerializeField, MustBeAssigned] private ItemGenerator itemGenerator = null;

    [Separator( "Basic attack" )]
    [SerializeField, MustBeAssigned] private GameObject energyProjectile = null;
    [SerializeField] private float energyShootPower = 10f;
    [SerializeField] private float timeBetweenEnergyShots = 0.5f;

    [Separator( "Effects" )]
    [SerializeField] private TelekinesisEffects effects = null;
    [SerializeField, MustBeAssigned] private GameObject counterAttackHint = null;
    [SerializeField, MustBeAssigned] private GameObject itemShootEffects = null;
    [SerializeField, MustBeAssigned] private GameObject energyShootEffects = null;
    [SerializeField, MustBeAssigned] private Vector2Event shockwaveEventSource = null;
    [SerializeField, MustBeAssigned] private FloatEvent shockwaveEventForce = null;
    [SerializeField] private float shockwaveForce = 5f;
    [SerializeField] private UnityEvent OnShoot = null;
    [SerializeField] private UnityEvent OnPull = null;
    [SerializeField] private UnityEvent OnRelease = null;

    [Separator( "Stable items" )]
    [SerializeField] private float stableItemFreezeTime = 5f;
    [SerializeField] private int maxStableItems = 5;

    [Separator( "Trajectory" )]
    [SerializeField, MustBeAssigned] private LineRenderer arcRenderer = null;
    [SerializeField] private int arcResolution = 10;
    [SerializeField] private float arcLength = 8f;

    [Separator( "Masks" )]
    [Tooltip( "Layers of all items player can pick up." )]
    public LayerMask itemsLayer;
    [Tooltip( "Layers of stable items player can modify." )]
    public LayerMask stableItemsLayer;
    [Tooltip( "Layers from where player can get rocks." )]
    public LayerMask rocksLayer;

    #endregion

    #region Private variables

    private GameObject closestItem;
    private float closestItemGravityScale;
    private GameObject closestStableItem;
    private InputController input;
    private PlayerState state;
    private List<GameObject> stableItems = new List<GameObject>();
    private float t = 0f;
    private bool canGetItemFromSurface = false;
    private bool isCursorInRange = false;
    private bool isCameraLeaning = false;
    private bool isCounterattacking = false;
    private bool canCounterAttack = false;
    private GameObject counterAttackItem = null;
    private float lastEnergyShootTime = 0f;
    private Queue<GameObject> inactiveEnergyProjectiles = new Queue<GameObject>();
    private List<GameObject> activeEnergyProjectiles = new List<GameObject>();

    #endregion


    public void RemoveStableItem()
    {
        stableItems.RemoveAt( 0 );
    }


    public void NotifyCounterAttackEnter( GameObject toCounter )
    {
        if ( !canCounterAttack && !state.isHoldingItemState && !state.isPullingItemState && !state.isAttackingState )
        {
            counterAttackItem = toCounter;
            canCounterAttack = true;
            counterAttackHint.SetActive( true );
        }
    }


    public void NotifyCounterAttackExit( GameObject toCounter )
    {
        if ( counterAttackItem == toCounter )
        {
            counterAttackItem = null;
            canCounterAttack = false;
            counterAttackHint.SetActive( false );
        }
    }


    private void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;
    }


    private void Update()
    {
        if ( TimeManager.instance.isGamePaused || state.isAttackingState || state.isDeadState )
        {
            return;
        }

        isCursorInRange = CheckCursorInRange();

        if ( isCursorInRange && !state.isHoldingItemState && !state.isPullingItemState )
        {
            FindItemsToPull();
        }
        else if ( effects.isCursorOver )
        {
            effects.SetCursorOver( false );
            if ( effects.areOverItemEffectsActive )
                effects.SetOverItemEffects( false );
            if ( effects.isInnerHighlightActive )
                effects.SetInnerHighlight( false );
            if ( effects.isOuterHighlightActive )
                effects.SetOuterHighlight( false );
            if ( !state.isHoldingItemState && !state.isPullingItemState )
            {
                closestItem?.GetComponent<Item>().OnHover( false );
                closestItem = null;
            }
        }

        if ( input.rmb )
        {
            if ( state.isHoldingItemState )
            {
                if ( closestItem.GetComponent<Item>().isSpawned )
                {
                    DestroySpawnedItem();
                }
                else
                {
                    ReleaseItem( closestItem );
                }
            }
            else if ( state.isPullingItemState )
            {
                closestItem.GetComponent<Item>().AbortPulling();
                ReleaseItem( closestItem );
            }
            else if ( isCursorInRange )
            {
                if ( closestItem != null )
                {
                    PullItem();
                }
                else if ( canGetItemFromSurface )
                {
                    PullItemFromSurface();
                }
                else
                {
                    // TODO: no item in range EFFECT (vfx, sfx)
                }
            }
        }
        else if ( input.spawnItem && !state.isHoldingItemState && !state.isPullingItemState )
        {
            if ( state.canSpawnItems && itemGenerator.selectedItem != null && EnergyController.instance.energy >= itemGenerator.itemSpawnCost )
            {
                SpawnItem();
            }
            else
            {
                // TODO: no item to spawn EFFECT (vfx, sfx)
            }
        }
        else if ( input.lmbDown )
        {
            if ( state.isHoldingItemState )
            {
                StartCoroutine( ShootingSequence() );
            }
            else if ( canCounterAttack && !state.isPullingItemState )
            {
                CounterAttack();
            }
        }
        else if ( input.lmbHold && !state.isAttackingState && Time.time - lastEnergyShootTime >= timeBetweenEnergyShots )
        {
            ShootEnergy();
            lastEnergyShootTime = Time.time;
        }

        // Player acquired stabling items skill
        /*if ( isCursorInRange && state.canModifyStableItems )
        {
            // Try to find stable item near cursor
            TryFindClosestStableItem();

            // Stable it
            if ( closestStableItem != null  && input.rmb )
            {
                SetNewStableItem();
            }
        }
        else
        {
            closestStableItem = null;
        }*/
    }


    private void FindItemsToPull()
    {
        GameObject previousClosestItem = closestItem;
        closestItem = FindClosestItem();

        if ( closestItem != null )
        {
            if ( closestItem != previousClosestItem )
                effects.closestItem = closestItem.transform;

            if ( !effects.areOverItemEffectsActive )
            {
                effects.SetOverItemEffects( true );
                if ( !effects.isCursorOver )
                    effects.SetCursorOver( true );
                if ( effects.isInnerHighlightActive )
                    effects.SetInnerHighlight( false );
                if ( effects.isOuterHighlightActive )
                    effects.SetOuterHighlight( false );
            }
        }
        else
        {
            effects.SetOverItemEffects( false );
            canGetItemFromSurface = FindRockySurface();
        }
    }


    private bool CheckCursorInRange()
    {
        bool result = Vector2.Distance( input.cursorPosition, transform.position ) > range ? false : true;

        if ( result != isCursorInRange )
        {
            CustomCursor.Instance?.SetInRange( result );
        }

        return result;
    }


    private void TryFindClosestStableItem()
    {
        closestStableItem = null;

        closestStableItem = Physics2D.OverlapCircle( input.cursorPosition, radius, stableItemsLayer ).gameObject;
    }


    private GameObject FindClosestItem()
    {
        GameObject result = null;

        Collider2D[] items = Physics2D.OverlapCircleAll( input.cursorPosition, radius, itemsLayer );

        float smallestDistance = Mathf.Infinity;

        for ( int i = 0; i < items.Length; i++ )
        {
            GameObject item = items[ i ].gameObject;

            float distanceToItem = Vector2.Distance( item.transform.position, input.cursorPosition );

            if ( distanceToItem < smallestDistance )
            {
                smallestDistance = distanceToItem;
                result = item;
            }
        }

        if ( result != closestItem)
        {
            closestItem?.GetComponent<Item>().OnHover( false );
            result?.GetComponent<Item>().OnHover( true );
        }

        return result;
    }


    private bool FindRockySurface()
    {
        Collider2D collider = Physics2D.OverlapCircle( input.cursorPosition, radius, rocksLayer );
        bool foundSurface = collider != null;

        if ( foundSurface )
        {
            if ( !effects.isCursorOver )
                effects.SetCursorOver( true );

            Vector2 closestPoint = collider.ClosestPoint( input.cursorPosition );
            Vector2 direction = closestPoint - input.cursorPosition;
            Vector2 normal = Physics2D.Raycast( input.cursorPosition, direction, Mathf.Infinity, rocksLayer ).normal;

            if ( normal.Approximately( Vector2.zero ) )
            {
                if ( !effects.isInnerHighlightActive )
                {
                    effects.SetInnerHighlight( true );
                    if ( effects.isOuterHighlightActive )
                        effects.SetOuterHighlight( false );
                }
            }
            else
            {
                effects.closestPoint = closestPoint;

                if ( !effects.isOuterHighlightActive )
                {
                    effects.SetOuterHighlight( true );
                    if ( effects.isInnerHighlightActive )
                        effects.SetInnerHighlight( false );
                    if ( effects.areOverItemEffectsActive )
                        effects.SetOverItemEffects( false );
                }
            }
        }
        else if ( effects.isCursorOver )
        {
            effects.SetCursorOver( false );
            if ( effects.isInnerHighlightActive )
                effects.SetInnerHighlight( false );
            if ( effects.isOuterHighlightActive )
                effects.SetOuterHighlight( false );
        }

        return foundSurface;
    }


    private void SpawnItem()
    {
        // TODO: Spawn effects
        closestItem = Instantiate( itemGenerator.selectedItem, holdingItemPlace.position, transform.rotation );
        Rigidbody2D closestItemRigidbody = closestItem.GetComponent<Rigidbody2D>();

        closestItem.GetComponent<Item>().OnHover( true );
        closestItem.GetComponent<Item>().isSpawned = true;

        EnergyController.instance.SubEnergy( itemGenerator.itemSpawnCost );

        state.isHoldingItemState = true;

        closestItemRigidbody.bodyType = RigidbodyType2D.Dynamic;
        closestItemRigidbody.isKinematic = false;
        closestItemRigidbody.simulated = false;

        closestItem.transform.SetParent( holdingItemPlace );

        canGetItemFromSurface = false;
    }


    private void PullItem()
    {
        OnPull.Invoke();

        closestItem.GetComponent<Item>().StartPulling( holdingItemPlace, pullSpeed, maxPullSpeed, this );

        state.isPullingItemState = true;

        effects.SetPullItemEffects( true );
    }


    private void PullItemFromSurface()
    {
        if ( EnergyController.instance.SubEnergy( pullFromSurfaceCost ) )
        {
            closestItem = Instantiate( 
                rocksToSpawn[ Random.Range( 0, rocksToSpawn.Length ) ], 
                input.cursorPosition, 
                transform.rotation );

            closestItem.GetComponent<Item>().OnHover( true );

            PullItem();
        }
        else
        {
            // TODO: not enough energy
        }
    }


    private void SetNewStableItem()
    {
        if ( stableItems.Count >= maxStableItems )
        {
            stableItems[ 0 ].GetComponent<StableItem>().SetUnstable( 0f );
        }

        StableItem stableItem = closestStableItem.GetComponent<StableItem>();
        stableItem.telekinesis = this;

        stableItems.Add( closestStableItem );
        stableItem.SetStable();
        stableItem.SetUnstable( stableItemFreezeTime );
    }


    private void CounterAttack()
    {
        Item item = counterAttackItem.GetComponent<Item>();
        item.enabled = true;
        item.MakeItem();
        isCounterattacking = true;
        EnergyController.instance.AddEnergy( EnergyGain.OnCounterAttack );
        StartCoroutine( ShootingSequence( counterAttackItem ) );
    }


    private IEnumerator ShootingSequence( GameObject item = null )
    {
        state.isAttackingState = true;
        arcRenderer.enabled = true;
        TimeManager.instance.TurnSlowmoOn();
        if ( item == null )
        {
            item = closestItem;
        }
        closestItemGravityScale = item.GetComponent<Rigidbody2D>().gravityScale;
        isCameraLeaning = true;

        while ( input.lmbHold && t < slowmoMaxTime )
        {
            LeanCameraTowardsCursor( t );
            ShowArc( item.transform );

            t += Time.unscaledDeltaTime;
            yield return null;
        }

        if ( isCounterattacking )
        {
            isCounterattacking = false;
            state.SetInvulnerable( true );
            state.SetInvulnerable( false, invulnerableTime );
        }
        isCameraLeaning = false;
        StartCoroutine( ZeroCameraOffset() );
        t = 0f;
        arcRenderer.enabled = false;
        TimeManager.instance.TurnSlowmoOff();
        ShootItem( item );
        state.isAttackingState = false;
        lastEnergyShootTime = Time.time;
    }


    private void LeanCameraTowardsCursor( float t )
    {
        Vector2 direction = input.cursorPosition - (Vector2)holdingItemPlace.position;
        if ( t < leanTime )
        {
            direction *= t * t / leanTime / leanTime;
        }

        cameraOffset.m_Offset = direction * leanWeight;
    }


    private IEnumerator ZeroCameraOffset()
    {
        while ( !isCameraLeaning && cameraOffset.m_Offset.magnitude > 0.01f)
        {
            cameraOffset.m_Offset *= zeroOffsetWeight;
            yield return null;
        }

        cameraOffset.m_Offset = Vector3.zero;
    }


    private void ShowArc( Transform from = null )
    {
        arcRenderer.positionCount = arcResolution + 1;
        arcRenderer.SetPositions( CalculateArcPositions( from ) );
    }


    private Vector3[] CalculateArcPositions( Transform from = null )
    {
        Vector3[] positions = new Vector3[ arcResolution + 1 ];

        if ( from == null )
        {
            from = holdingItemPlace;
        }

        Vector2 throwVector = input.cursorPosition - (Vector2)from.position;
        throwVector = throwVector.normalized * arcLength;
        float radianAngle = Mathf.Atan2( throwVector.y, throwVector.x );

        for ( int i = 0; i <= arcResolution; i++ )
        {
            float t = (float)i / (float)arcResolution * Mathf.Sign( throwVector.x );
            positions[ i ] = CalculateArcPoint( t, throwVector, radianAngle, from );
        }

        return positions;
    }


    private Vector3 CalculateArcPoint( float t, Vector2 throwVector, float radianAngle, Transform from )
    {
        float x = from.position.x + t * Mathf.Abs( throwVector.x );
        float xy = t * Mathf.Abs( throwVector.x );
        // y = y0 + x*tg - g*x^2 / (2 * F^2 * cos^2)
        float y = from.position.y + xy * Mathf.Tan( radianAngle ) - -Physics2D.gravity.y * closestItemGravityScale * xy * xy / 2f / shootPower / shootPower / Mathf.Cos( radianAngle ) / Mathf.Cos( radianAngle );
        return new Vector3( x, y );
    }


    private void ShootItem( GameObject item )
    {
        OnShoot.Invoke();

        Vector2 shootDirection = input.cursorPosition - (Vector2)item.transform.position;

        float angle = Mathf.Atan2( shootDirection.y, shootDirection.x ) * Mathf.Rad2Deg;

        Instantiate( itemShootEffects, item.transform.position, Quaternion.AngleAxis( angle, Vector3.forward ));

        item.GetComponent<Item>().Shoot( shootDirection.normalized, shootPower );

        shockwaveEventSource.Broadcast( gameObject, holdingItemPlace.position );
        shockwaveEventForce.Broadcast( gameObject, shockwaveForce );

        ReleaseItem( item );
    }


    private IEnumerator EnqueueEnergyProjectile( GameObject projectile )
    {
        yield return new WaitForSeconds( 0.1f );
        inactiveEnergyProjectiles.Enqueue( projectile );
    }


    private void ShootEnergy()
    {
        GameObject proj = null;

        for ( int i = 0; i < activeEnergyProjectiles.Count; i++ )
        {
            proj = activeEnergyProjectiles[ i ];
            if ( !proj.activeSelf )
            {
                StartCoroutine( EnqueueEnergyProjectile( proj ) );
                activeEnergyProjectiles.RemoveAt( i-- );
            }
        }

        if ( inactiveEnergyProjectiles.Count == 0 )
        {
            proj = Instantiate( energyProjectile, holdingItemPlace.position, Quaternion.identity, null );
        }
        else
        {
            proj = inactiveEnergyProjectiles.Dequeue();
            proj.transform.position = holdingItemPlace.position;
        }

        activeEnergyProjectiles.Add( proj );

        Vector2 shootDirection = input.cursorPosition - (Vector2)holdingItemPlace.position;

        float angle = Mathf.Atan2( shootDirection.y, shootDirection.x ) * Mathf.Rad2Deg;

        Instantiate( energyShootEffects, holdingItemPlace.position, Quaternion.AngleAxis( angle, Vector3.forward ) );

        proj.GetComponent<EnergyProjectile>().Shoot( shootDirection.normalized, energyShootPower );

        state.isPullingItemState = false;
        state.isHoldingItemState = false;
    }


    private void ReleaseItem( GameObject item )
    {
        OnRelease.Invoke();

        if ( item == closestItem )
        {
            closestItem.GetComponent<Item>().SetFree();
            if ( !state.isAttackingState )
            {
                closestItem.GetComponent<Item>().OnHover( false );
            }
            effects.SetPullItemEffects( false );
            closestItem = null;
        }
        else
        {
            NotifyCounterAttackExit( item );
        }

        state.isPullingItemState = false;
        state.isHoldingItemState = false;
    }


    private void DestroySpawnedItem()
    {
        // TODO: Destroy effects
        state.isPullingItemState = false;
        state.isHoldingItemState = false;
        Destroy( closestItem );
    }
}