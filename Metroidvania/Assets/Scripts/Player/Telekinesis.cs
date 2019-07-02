using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Telekinesis : MonoBehaviour
{
    #region Editor variables

    [Header("Telekinesis")]
    [SerializeField] private float range = 10f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float pullSpeed = 50f;
    [SerializeField] private float maxPullSpeed = 50f;
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private float slowmoMaxTime = 2f;
    [SerializeField] private Transform holdingItemPlace = null;
    [SerializeField] private GameObject rockToSpawn = null;

    [Header("Effects")]
    [SerializeField] private Light _light = null;
    [SerializeField] private float lightOnIntensity = 22f;
    [SerializeField] private float lightOffIntensity = 10f;
    [SerializeField] private SpriteRenderer lightCircle = null;
    [SerializeField] private Color lightCircleOnColor = Color.clear;
    [SerializeField] private Color lightCircleOffColor = Color.clear;
    [SerializeField] private float tweenTime = 0.5f;
    [SerializeField] private ParticleSystem objectToPullHighlightParticles = null;
    [SerializeField] private GameObject objectToPullHighlight = null;
    [SerializeField] private GameObject pullEffects = null;
    [SerializeField] private GameObject shootEffects = null;
    [SerializeField] private UnityEvent OnShoot = null;
    [SerializeField] private Transform anchor = null;
    [SerializeField] private Transform anchor2 = null;
    [SerializeField] private GameObject shootEffectBlur = null;
    [SerializeField] private float shootEffectBlurSize = 1.5f;
    [SerializeField] private float shootEffectBlurTime = 0.1f;

    [Header("Stable items")]
    [SerializeField] private float stableItemFreezeTime = 5f;
    [SerializeField] private int maxStableItems = 5;

    [Header("Trajectory")]
    [SerializeField] private LineRenderer arcRenderer = null;
    [SerializeField] private int arcResolution = 10;
    [SerializeField] private float arcLength = 8f;

    [Header("Masks")]
    [Tooltip("Layers of all items player can pick up.")]
    public LayerMask itemsLayer;
    [Tooltip("Layers from where player can get rocks.")]
    public LayerMask rocksLayer;

    #endregion

    #region Private variables

    private GameObject closestItem;
    private Rigidbody2D closestItemRigidbody;
    private GameObject closestStableItem;
    private InputController input;
    private PlayerState state;
    private bool isHoldingLMB;
    private string stableItemsTag = "StableItem";
    private List<GameObject> stableItems = new List<GameObject>();
    private float t = 0f;
    private bool canGetRockFromGround = false;

    #endregion

    private void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;
        _light.intensity = lightOffIntensity;
        lightCircle.color = lightCircleOffColor;
    }

    private void Update()
    {
        CheckForItems();

        HighlightObjectToPull();

        if (input.rmb)
        {
            CheckItem();

            CheckStableItem();
        }

        CheckShoot();

        //temporary
        if (state.isHoldingItemState)
        {
            pullEffects.SetActive(false);
        }
    }

    
    private void CheckForItems()
    {
        if (!state.isHoldingItemState && !state.isPullingItemState)
        {
            closestItem = null;
        }

        closestStableItem = null;
        canGetRockFromGround = false;

        if (Vector2.Distance(input.cursorPosition, transform.position) < range)
        {
            FindClosestItem();
        }
    }

    private void HighlightObjectToPull()
    {
        if (state.isHoldingItemState || state.isPullingItemState)
        {
            objectToPullHighlight.SetActive(false);
        }
        else
        {
            if (closestItem)
            {
                if (!objectToPullHighlightParticles.isEmitting)
                    objectToPullHighlight.SetActive(true);
                Vector3 pos = new Vector3(closestItem.transform.position.x, closestItem.transform.position.y, closestItem.transform.position.z + 0.1f);
                objectToPullHighlight.transform.position = pos;
            }
            else if (canGetRockFromGround)
            {
                if (!objectToPullHighlightParticles.isEmitting)
                    objectToPullHighlight.SetActive(true);
                objectToPullHighlight.transform.position = input.cursorPosition;
            }
            else
            {
                objectToPullHighlight.SetActive(false);
            }
        }
    }

    private void FindClosestItem()
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(input.cursorPosition, radius, itemsLayer);

        float smallestDistance = Mathf.Infinity;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].CompareTag(stableItemsTag))
            {
                closestStableItem = items[i].gameObject;
            }
            else if (!state.isHoldingItemState && !state.isPullingItemState)
            {
                float distanceToItem = Vector2.Distance(items[i].transform.position, input.cursorPosition);
                if (distanceToItem < smallestDistance)
                {
                    smallestDistance = distanceToItem;
                    closestItem = items[i].gameObject;
                    closestItemRigidbody = closestItem.GetComponent<Rigidbody2D>();
                }
            }
        }

        if (closestItem == null && Physics2D.OverlapCircle(input.cursorPosition, radius, rocksLayer))
        {
            canGetRockFromGround = true;
        }
    }


    private void PullItemFromGround()
    {
        closestItem = Instantiate(rockToSpawn, input.cursorPosition, transform.rotation);
        closestItemRigidbody = closestItem.GetComponent<Rigidbody2D>();
        PullItem();
    }


    private void PullItem()
    {
        closestItem.GetComponent<ItemHandling>().PullItem(holdingItemPlace, pullSpeed, maxPullSpeed);
        SetPullEffectsActive(true);
        pullEffects.SetActive(true);
    }


    private void CheckItem()
    {
        if (!state.isHoldingItemState &&
            !state.isPullingItemState)
        {
            if (closestItem != null && !closestItem.CompareTag(stableItemsTag))
            {
                PullItem();
            }
            else if (canGetRockFromGround)
            {
                PullItemFromGround();
            }
        }
        else if ((state.isHoldingItemState || state.isPullingItemState) && !isHoldingLMB)
        {
            if (state.isPullingItemState)
            {
                closestItem.GetComponent<ItemHandling>().StopPulling();
            }

            ReleaseItem();
        }
    }


    private void CheckStableItem()
    {
        if (closestStableItem != null)
        {
            if (stableItems.Count >= maxStableItems)
            {
                SetStableItem(stableItems[0], false);
            }
            SetStableItem(closestStableItem, true, stableItemFreezeTime);
        }
    }


    private void ShowArc()
    {
        arcRenderer.positionCount = arcResolution + 1;
        arcRenderer.SetPositions(CalculateArcPositions());
    }


    private Vector3[] CalculateArcPositions()
    {
        Vector3[] positions = new Vector3[arcResolution + 1];

        Vector2 throwVector = input.cursorPosition - (Vector2)holdingItemPlace.position;
        throwVector = throwVector.normalized * arcLength;
        float radianAngle = Mathf.Atan2(throwVector.y, throwVector.x);

        for (int i = 0; i <= arcResolution; i++)
        {
            float t = (float)i / (float)arcResolution * Mathf.Sign(throwVector.x);
            positions[i] = CalculateArcPoint(t, throwVector, radianAngle);
        }

        return positions;
    }


    private Vector3 CalculateArcPoint(float t, Vector2 throwVector, float radianAngle)
    {
        float x = holdingItemPlace.position.x + t * Mathf.Abs(throwVector.x);
        float xy = t * Mathf.Abs(throwVector.x);
        // y = y0 + x*tg - g*x^2 / (2 * F^2 * cos^2)
        float y = holdingItemPlace.position.y + xy * Mathf.Tan(radianAngle) - -Physics2D.gravity.y * closestItemRigidbody.gravityScale * xy * xy / 2f / shootPower / shootPower / Mathf.Cos(radianAngle) / Mathf.Cos(radianAngle);
        return new Vector3(x, y);
    }


    private void CheckShoot()
    {
        if (state.isHoldingItemState)
        {
            if (input.lmbDown)
            {
                arcRenderer.enabled = true;
                ShowArc();
                TimeManager.instance.TurnSlowmoOn();
                isHoldingLMB = true;
            }
            else if (isHoldingLMB)
            {
                ShowArc();
                if (input.lmbUp || t >= slowmoMaxTime)
                {
                    arcRenderer.enabled = false;
                    TimeManager.instance.TurnSlowmoOff();
                    ShootItem();
                    isHoldingLMB = false;
                    t = 0f;
                }
                else
                {
                    t += Time.unscaledDeltaTime;
                }
            }
        }
    }


    private void ShootItem()
    {
        OnShoot.Invoke();

        float tmp = anchor.rotation.eulerAngles.z;
        tmp = tmp > 180f ? tmp - 360f : tmp;
        if (tmp > -50f)
        {
            anchor.eulerAngles = new Vector3(0f, 0f, -50f);
            anchor2.eulerAngles = new Vector3(0f, 0f, -50f);
        }

        LeanTween.value(shootEffectBlur, new Vector2(0f, 0f), new Vector2(shootEffectBlurSize, shootEffectBlurSize), shootEffectBlurTime).setOnUpdate((Vector2 v) => shootEffectBlur.transform.localScale = v).setOnComplete(() => shootEffectBlur.transform.localScale = new Vector2(0f, 0f));

        Vector2 shootDirection = input.cursorPosition - (Vector2)holdingItemPlace.position;
        shootDirection.Normalize();
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        shootEffects.transform.eulerAngles = Vector3.forward * angle;
        ReleaseItem();
        closestItem.GetComponent<ItemShoot>().Shoot(shootDirection, shootPower);
    }


    private void SetStableItem(GameObject go, bool toStable, float time = 0f)
    {
        StableItemHandling sih = go.GetComponent<StableItemHandling>();
        sih.telekinesis = this;

        if (toStable)
        {
            stableItems.Add(go);
            sih.SetStable();
            sih.SetUnstable(time);
        }
        else
        {
            sih.SetUnstable(time);
        }
    }


    private void ReleaseItem()
    {
        closestItem.GetComponent<ItemHandling>().SetFree();
        state.isPullingItemState = false;
        state.isHoldingItemState = false;
        SetPullEffectsActive(false);
    }


    private void SetPullEffectsActive(bool on)
    {
        LeanTween.value(_light.gameObject, _light.intensity, on ? lightOnIntensity : lightOffIntensity, tweenTime).setOnUpdate((float v) => _light.intensity = v);
        LeanTween.value(_light.gameObject, lightCircle.color, on ? lightCircleOnColor : lightCircleOffColor, tweenTime).setOnUpdate((Color c) => lightCircle.color = c);
    }


    public void RemoveStableItem()
    {
        stableItems.RemoveAt(0);
    }
}
