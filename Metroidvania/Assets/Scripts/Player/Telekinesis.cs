using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    #region Editor variables

    [Header("Telekinesis")]
    [SerializeField] private float range = 10f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float pullForce = 75f;
    [SerializeField] private float maxPullForce = 50f;
    [SerializeField] private float pullSpeed = 50f;
    [SerializeField] private float maxPullSpeed = 50f;
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private float slowmoMaxTime = 2f;
    [SerializeField] private Transform holdingItemPlace = null;

    [Header("Stable items")]
    [SerializeField] private float stableItemFreezeTime = 5f;
    [SerializeField] private int maxStableItems = 5;

    [Header("Trajectory")]
    [SerializeField] private LineRenderer arcRenderer = null;
    [SerializeField] private int arcResolution = 10;
    [SerializeField] private float arcLength = 8f;

    [Header("Collision masks")]
    public LayerMask itemsLayer;

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

    #endregion

    void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;
    }

    void Update()
    {
        CheckForItems();

        if (input.rmb)
        {
            CheckItem();

            CheckStableItem();
        }

        CheckShoot();
    }

    
    private void CheckForItems()
    {
        if (!state.isHoldingItemState && !state.isPullingItemState)
            closestItem = null;

        closestStableItem = null;

        if (Vector2.Distance(input.cursorPosition, transform.position) < range)
        {
            FindClosestItem();

            // Light up closest item
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
    }


    private void CheckItem()
    {
        if (!state.isHoldingItemState &&
            !state.isPullingItemState &&
            closestItem != null &&
            !closestItem.CompareTag(stableItemsTag))
        {
            closestItem.AddComponent<ItemPull>().Pull(holdingItemPlace, pullForce, maxPullForce, pullSpeed, maxPullSpeed);
        }
        else if (state.isHoldingItemState || state.isPullingItemState)
        {
            if (state.isPullingItemState)
            {
                closestItem.GetComponent<ItemPull>().StopPulling();
            }

            ReleaseItem();
        }
    }


    private void CheckStableItem()
    {
        if (closestStableItem != null)
        {
            if (stableItems.Count < maxStableItems)
            {
                SetStableItem(closestStableItem, true, stableItemFreezeTime);
            }
            else
            {
                SetStableItem(stableItems[0], false);
                SetStableItem(closestStableItem, true, stableItemFreezeTime);
            }
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
        Vector2 shootDirection = input.cursorPosition - (Vector2)holdingItemPlace.position;
        shootDirection.Normalize();
        ReleaseItem();
        closestItem.GetComponent<Rigidbody2D>().AddForce(shootDirection * shootPower, ForceMode2D.Impulse);
        closestItem.GetComponent<ItemShoot>().isShooted = true;
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
        closestItem.transform.SetParent(null);
        Rigidbody2D closestItemRigidbody = closestItem.GetComponent<Rigidbody2D>();
        if (state.isHoldingItemState)
        {
            closestItemRigidbody.velocity = Vector2.zero;
            closestItemRigidbody.angularVelocity = 0f;
        }
        closestItemRigidbody.simulated = true;
        state.isPullingItemState = false;
        state.isHoldingItemState = false;
    }


    public void RemoveStableItem()
    {
        stableItems.RemoveAt(0);
    }
}
