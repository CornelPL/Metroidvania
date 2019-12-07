using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.LWRP;
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
    [SerializeField, MustBeAssigned] private CinemachineCameraOffset cameraOffset = null;
    [SerializeField, MustBeAssigned] private Transform holdingItemPlace = null;
    [SerializeField, MustBeAssigned] private GameObject rockToSpawn = null;
    [SerializeField, MustBeAssigned] private ItemGenerator itemGenerator = null;

    [Separator( "Effects" )]
    [SerializeField, MustBeAssigned] private Light2D _light = null;
    [SerializeField] private float lightOnIntensity = 1.5f;
    [SerializeField] private float lightOffIntensity = 0.5f;
    [SerializeField] private float tweenTime = 0.5f;
    [SerializeField, MustBeAssigned] private GameObject pullEffects = null;
    [SerializeField, MustBeAssigned] private ParticleSystem onOverItemParticles = null;
    [SerializeField, MustBeAssigned] private GameObject itemHighlight = null;
    [SerializeField, MustBeAssigned] private GameObject shootEffects = null;
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
    private bool isCursorOver = false;
    private bool isCameraLeaning = false;

    #endregion


    public void RemoveStableItem()
    {
        stableItems.RemoveAt( 0 );
    }


    private void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;
        _light.intensity = lightOffIntensity;
    }


    private void Update()
    {
        if ( TimeManager.instance.isGamePaused )
        {
            return;
        }

        isCursorInRange = CheckCursorInRange();

        if ( isCursorInRange && !state.isHoldingItemState && !state.isPullingItemState )
        {
            FindItemsToPull();
        }

        CheckOverItemEffects();
        
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
                    ReleaseItem();
                }
            }
            else if ( state.isPullingItemState )
            {
                closestItem.GetComponent<Item>().AbortPulling();
                ReleaseItem();
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
        else if ( input.lmbDown && state.isHoldingItemState )
        {
            StartCoroutine( ShootingSequence() );
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
        closestItem = FindClosestItem();

        if ( closestItem == null )
        {
            canGetItemFromSurface = FindRockySurface();
        }
        else
        {
            canGetItemFromSurface = false;
            itemHighlight.SetActive( false );
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


    private void SetOverItemEffects( bool on )
    {
        isCursorOver = on;
        itemHighlight.SetActive( on );
        CustomCursor.Instance?.SetOver( on );

        if ( on == false && onOverItemParticles.isPlaying )
        {
            onOverItemParticles.Stop();
            onOverItemParticles.Clear();
        }
        else if ( on == true && !onOverItemParticles.isPlaying )
        {
            onOverItemParticles.Play();
        }
    }


    private void CheckOverItemEffects()
    {
        if ( !state.isHoldingItemState && !state.isPullingItemState )
        {
            if ( closestItem != null )
            {
                if ( !isCursorOver )
                {
                    SetOverItemEffects( true );
                    itemHighlight.SetActive( false );
                }

                onOverItemParticles.transform.position = closestItem.transform.position;
            }
            else if ( isCursorOver && ( !canGetItemFromSurface || !isCursorInRange ) )
            {
                SetOverItemEffects( false );
            }
            else if ( canGetItemFromSurface && isCursorInRange )
            {
                if ( !isCursorOver )
                {
                    SetOverItemEffects( true );
                }

                onOverItemParticles.transform.position = input.cursorPosition;
            }
        }
        
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
        bool result = Physics2D.OverlapCircle( input.cursorPosition, radius, rocksLayer ) != null;
        
        if ( result == true && !canGetItemFromSurface )
        {
            itemHighlight.SetActive( true );
        }

        return result;
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

        SetOverItemEffects( false );

        closestItem.GetComponent<Item>().StartPulling( holdingItemPlace, pullSpeed, maxPullSpeed );

        state.isPullingItemState = true;

        SetPullEffectsActive( true );
    }


    private void PullItemFromSurface()
    {
        // TODO: select rockToSpawn from list of rocks
        closestItem = Instantiate( rockToSpawn, input.cursorPosition, transform.rotation );

        closestItem.GetComponent<Item>().OnHover( true );

        PullItem();
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


    private IEnumerator ShootingSequence()
    {
        SetPullEffectsActive( false );
        arcRenderer.enabled = true;
        TimeManager.instance.TurnSlowmoOn();
        closestItemGravityScale = closestItem.GetComponent<Rigidbody2D>().gravityScale;
        isCameraLeaning = true;

        while ( !input.lmbUp && t < slowmoMaxTime )
        {
            LeanCameraTowardsCursor( t );
            ShowArc();

            t += Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }

        isCameraLeaning = false;
        StartCoroutine( ZeroCameraOffset() );
        t = 0f;
        arcRenderer.enabled = false;
        TimeManager.instance.TurnSlowmoOff();
        ShootItem();
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
            yield return new WaitForEndOfFrame();
        }

        cameraOffset.m_Offset = Vector3.zero;
    }


    private void ShowArc()
    {
        arcRenderer.positionCount = arcResolution + 1;
        arcRenderer.SetPositions( CalculateArcPositions() );
    }


    private Vector3[] CalculateArcPositions()
    {
        Vector3[] positions = new Vector3[ arcResolution + 1 ];

        Vector2 throwVector = input.cursorPosition - (Vector2)holdingItemPlace.position;
        throwVector = throwVector.normalized * arcLength;
        float radianAngle = Mathf.Atan2( throwVector.y, throwVector.x );

        for ( int i = 0; i <= arcResolution; i++ )
        {
            float t = (float)i / (float)arcResolution * Mathf.Sign( throwVector.x );
            positions[ i ] = CalculateArcPoint( t, throwVector, radianAngle );
        }

        return positions;
    }


    private Vector3 CalculateArcPoint( float t, Vector2 throwVector, float radianAngle )
    {
        float x = holdingItemPlace.position.x + t * Mathf.Abs( throwVector.x );
        float xy = t * Mathf.Abs( throwVector.x );
        // y = y0 + x*tg - g*x^2 / (2 * F^2 * cos^2)
        float y = holdingItemPlace.position.y + xy * Mathf.Tan( radianAngle ) - -Physics2D.gravity.y * closestItemGravityScale * xy * xy / 2f / shootPower / shootPower / Mathf.Cos( radianAngle ) / Mathf.Cos( radianAngle );
        return new Vector3( x, y );
    }


    private void ShootItem()
    {
        OnShoot.Invoke();

        Vector2 shootDirection = input.cursorPosition - (Vector2)holdingItemPlace.position;

        float angle = Mathf.Atan2( shootDirection.y, shootDirection.x ) * Mathf.Rad2Deg;

        Instantiate( shootEffects, holdingItemPlace.position, Quaternion.AngleAxis( angle, Vector3.forward ));

        closestItem.GetComponent<Item>().Shoot( shootDirection.normalized, shootPower );

        shockwaveEventSource.Broadcast( gameObject, holdingItemPlace.position );
        shockwaveEventForce.Broadcast( gameObject, shockwaveForce );

        ReleaseItem();
    }


    private void ReleaseItem()
    {
        OnRelease.Invoke();

        closestItem.GetComponent<Item>().SetFree();
        closestItem.GetComponent<Item>().OnHover( false );
        closestItem = null;
        state.isPullingItemState = false;
        state.isHoldingItemState = false;
        SetPullEffectsActive( false );
    }


    private void DestroySpawnedItem()
    {
        // TODO: Destroy effects
        state.isPullingItemState = false;
        state.isHoldingItemState = false;
        SetPullEffectsActive( false );
        Destroy( closestItem );
    }


    private void SetPullEffectsActive( bool on )
    {
        pullEffects.SetActive( on );

        if ( !state.isHoldingItemState )
        {
            LeanTween.value( _light.gameObject, _light.intensity, on ? lightOnIntensity : lightOffIntensity, tweenTime ).setOnUpdate( ( float v ) => _light.intensity = v );
        }
    }
}