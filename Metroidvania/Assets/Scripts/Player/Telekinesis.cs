using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float pullForce = 75f;
    [SerializeField] private float maxPullForce = 50f;
    [SerializeField] private float pullSpeed = 50f;
    [SerializeField] private float maxPullSpeed = 50f;
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private float slowmoMaxTime = 2f;
    [SerializeField] private float stableItemFreezeTime = 5f;
    [SerializeField] private int maxStableItems = 5;
    [SerializeField] private Transform holdingItemPlace = null;
    public LayerMask itemsLayer;
    public LayerMask collisionLayer;

    private GameObject closestItem;
    private GameObject closestStableItem;
    private InputController input;
    private PlayerState state;
    private bool isHoldingLMB;
    private string stableItemsTag = "StableItem";
    private List<GameObject> stableItems = new List<GameObject>();
    private float t = 0f;

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
            closestItem.AddComponent<ItemHandling>().Pull(holdingItemPlace, pullForce, maxPullForce, pullSpeed, maxPullSpeed, collisionLayer);
            state.isPullingItemState = true;
        }
        else if (state.isHoldingItemState || state.isPullingItemState)
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


    private void CheckShoot()
    {
        if (state.isHoldingItemState)
        {
            if (input.lmbDown)
            {
                TimeManager.instance.TurnSlowmoOn();
                isHoldingLMB = true;
            }
            else if (isHoldingLMB)
            {
                if (input.lmbUp || t >= slowmoMaxTime)
                {
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
