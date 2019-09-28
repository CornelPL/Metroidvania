using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Experimental.Rendering.LWRP;

public class Telekinesis : MonoBehaviour
{
    #region Editor variables

    [Header( "Telekinesis" )]
    [SerializeField] private float range = 10f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float pullSpeed = 50f;
    [SerializeField] private float maxPullSpeed = 50f;
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private float slowmoMaxTime = 2f;
    [SerializeField] private Transform holdingItemPlace = null;
    [SerializeField] private GameObject rockToSpawn = null;

    [Header( "Effects" )]
    [SerializeField] private Light2D _light = null;
    [SerializeField] private float lightOnIntensity = 1.5f;
    [SerializeField] private float lightOffIntensity = 0.5f;
    [SerializeField] private float tweenTime = 0.5f;
    [SerializeField] private GameObject pullEffects = null;
    [SerializeField] private GameObject shootEffects = null;
    [SerializeField] private UnityEvent OnShoot = null;
    [SerializeField] private GameObject shootEffectBlur = null;
    [SerializeField] private float shootEffectBlurSize = 1.5f;
    [SerializeField] private float shootEffectBlurTime = 0.1f;

    [Header( "Stable items" )]
    [SerializeField] private float stableItemFreezeTime = 5f;
    [SerializeField] private int maxStableItems = 5;

    [Header( "Trajectory" )]
    [SerializeField] private LineRenderer arcRenderer = null;
    [SerializeField] private int arcResolution = 10;
    [SerializeField] private float arcLength = 8f;

    [Header( "Masks" )]
    [Tooltip( "Layers of all items player can pick up." )]
    public LayerMask itemsLayer;
    [Tooltip( "Layers of stable items player can modify." )]
    public LayerMask stableItemsLayer;
    [Tooltip( "Layers from where player can get rocks." )]
    public LayerMask rocksLayer;

    #endregion

    #region Private variables

    private GameObject closestItem;
    private Rigidbody2D closestItemRigidbody;
    private GameObject closestStableItem;
    private InputController input;
    private PlayerState state;
    private bool isHoldingLMB;
    private List<GameObject> stableItems = new List<GameObject>();
    private float t = 0f;
    private bool canGetRockFromGround = false;

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
        if ( !state.isHoldingItemState && !state.isPullingItemState )
        {
            TryFindClosestItem();

            TryHighlightItemToPull();

            if ( input.rmb )
            {
                if ( closestItem != null )
                {
                    PullItem();
                }
                else if ( canGetRockFromGround )
                {
                    PullItemFromGround();
                }
            }
        }
        else if ( input.rmb )
        {
            if ( state.isPullingItemState )
            {
                closestItem.GetComponent<Item>().AbortPulling();
            }

            ReleaseItem();
        }

        if ( state.canModifyStableItems )
        {
            TryFindClosestStableItem();

            if ( closestStableItem != null)
            {
                HighlightStableItem();

                if ( input.rmb )
                {
                    SetNewStableItem();
                }
            }
        }

        if ( state.isHoldingItemState )
        {
            if ( input.lmbDown )
            {
                StartShootingSequence();
            }
            else if ( isHoldingLMB )
            {
                ShootingSequence();
            }
        }

        if ( state.isHoldingItemState )
        {
            SetPullEffectsActive( false );
        }
    }


    private void TryFindClosestStableItem()
    {
        closestStableItem = null;

        if ( Vector2.Distance( input.cursorPosition, transform.position ) < range )
        {
            closestStableItem = Physics2D.OverlapCircle( input.cursorPosition, radius, stableItemsLayer ).gameObject;
        }
    }


    private void TryFindClosestItem()
    {
        closestItem = null;
        canGetRockFromGround = false;

        if ( Vector2.Distance( input.cursorPosition, transform.position ) > range )
        {
            return;
        }

        Collider2D[] items = Physics2D.OverlapCircleAll( input.cursorPosition, radius, itemsLayer );

        float smallestDistance = Mathf.Infinity;

        for ( int i = 0; i < items.Length; i++ )
        {
            GameObject item = items[ i ].gameObject;

            float distanceToItem = Vector2.Distance( item.transform.position, input.cursorPosition );
            if ( distanceToItem < smallestDistance )
            {
                smallestDistance = distanceToItem;
                closestItem = item;
            }
        }

        closestItemRigidbody = closestItem?.GetComponent<Rigidbody2D>();

        if ( closestItem == null && Physics2D.OverlapCircle( input.cursorPosition, radius, rocksLayer ) )
        {
            canGetRockFromGround = true;
        }
    }


    private void TryHighlightItemToPull()
    {
        // TODO
        if ( closestItem != null )
        {
            // highlight closest item
        }
        else if ( canGetRockFromGround )
        {
            // highlight at input.cursorPosition
        }
        else
        {
            // turn highlight off
        }
    }


    private void HighlightStableItem()
    {
        // TODO
    }


    private void PullItemFromGround()
    {
        closestItem = Instantiate( rockToSpawn, input.cursorPosition, transform.rotation );
        closestItemRigidbody = closestItem.GetComponent<Rigidbody2D>();

        PullItem();
    }


    private void PullItem()
    {
        closestItem.GetComponent<Item>().StartPulling( holdingItemPlace, pullSpeed, maxPullSpeed );

        SetPullEffectsActive( true );
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


    private void StartShootingSequence()
    {
        arcRenderer.enabled = true;
        ShowArc();
        TimeManager.instance.TurnSlowmoOn();
        isHoldingLMB = true;
    }


    private void ShootingSequence()
    {
        ShowArc();

        if ( input.lmbUp || t >= slowmoMaxTime )
        {
            arcRenderer.enabled = false;
            TimeManager.instance.TurnSlowmoOff();
            isHoldingLMB = false;
            t = 0f;

            ShootItem();
        }
        else
        {
            t += Time.unscaledDeltaTime;
        }
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
        float y = holdingItemPlace.position.y + xy * Mathf.Tan( radianAngle ) - -Physics2D.gravity.y * closestItemRigidbody.gravityScale * xy * xy / 2f / shootPower / shootPower / Mathf.Cos( radianAngle ) / Mathf.Cos( radianAngle );
        return new Vector3( x, y );
    }


    private void ShootItem()
    {
        OnShoot.Invoke();

        LeanTween.value( shootEffectBlur, new Vector3( 0f, 0f, 1f ), new Vector3( shootEffectBlurSize, shootEffectBlurSize, 1f ), shootEffectBlurTime ).setOnUpdate( ( Vector3 v ) => shootEffectBlur.transform.localScale = v ).setOnComplete( () => shootEffectBlur.transform.localScale = new Vector3( 0f, 0f, 1f ) );

        SpriteRenderer renderer = shootEffectBlur.GetComponent<SpriteRenderer>();

        LeanTween.value( shootEffectBlur, 1f, 0f, shootEffectBlurTime ).setOnUpdate( ( float f ) => renderer.color = new Color( renderer.color.r, renderer.color.g, renderer.color.b, f ) );

        Vector2 shootDirection = input.cursorPosition - (Vector2)holdingItemPlace.position;
        shootDirection.Normalize();

        float angle = Mathf.Atan2( shootDirection.y, shootDirection.x ) * Mathf.Rad2Deg;

        shootEffects.transform.eulerAngles = Vector3.forward * angle;

        ReleaseItem();

        closestItem.GetComponent<Item>().Shoot( shootDirection, shootPower );
    }


    private void ReleaseItem()
    {
        closestItem.GetComponent<Item>().SetFree();
        state.isPullingItemState = false;
        state.isHoldingItemState = false;
        SetPullEffectsActive( false );
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