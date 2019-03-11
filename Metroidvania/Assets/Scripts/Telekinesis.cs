using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float pullSpeed = 2f;
    [SerializeField] private float maxPullSpeed = 20f;
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private float itemFreezeTime = 10f;
    [SerializeField] private int maxStableItems = 5;
    [SerializeField] private Transform holdingItemPlace = null;
    [SerializeField] private string itemsLayerS = "Items";
    [SerializeField] private string groundLayerS = "Ground";
    public LayerMask itemsLayer;

    private Camera _camera;
    private GameObject closestItem;
    private InputController input;
    private PlayerState state;
    private Vector2 cursorPosition;
    private bool isHoldingLMB;
    private string stableItemsTag = "StableItem";
    private List<GameObject> stableItems = new List<GameObject>();

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        input = InputController.instance;
        state = PlayerState.instance;
    }

    void Update()
    {
        cursorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (!state.isHoldingItemState && !state.isPullingItemState)
        {
            if (Vector2.Distance(cursorPosition, transform.position) < range)
            {
                FindClosestItem();

                // Light up closest item
            }

            if (closestItem != null && input.rmb)
            {
                closestItem.AddComponent<ItemHandling>().Pull(holdingItemPlace, pullSpeed, maxPullSpeed);
            }
        }

        if (state.isHoldingItemState)
        {
            if (input.lmbDown)
            {
                TimeManager.instance.TurnSlowmoOn();
                isHoldingLMB = true;
            }
            else if (input.lmbUp && isHoldingLMB)
            {
                TimeManager.instance.TurnSlowmoOff();
                ShootItem();
                isHoldingLMB = false;
            }
            else if (input.rmb)
            {
                ReleaseItem();                

                if (closestItem.CompareTag(stableItemsTag))
                {
                    if(stableItems.Count < maxStableItems)
                    {
                        SetStableItem(closestItem, true, itemFreezeTime);
                    }
                    else
                    {
                        SetStableItem(stableItems[0], false);
                        SetStableItem(closestItem, true, itemFreezeTime);
                    }
                }
            }
        }
    }

    void FindClosestItem()
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(cursorPosition, radius, itemsLayer);

        if (items.Length == 0)
        {
            closestItem = null;
            return;
        }

        float smallestDistance = Mathf.Infinity;

        for (int i = 0; i < items.Length; i++)
        {
            float distanceToItem = Vector2.Distance(items[i].transform.position, transform.position);
            if (distanceToItem < smallestDistance)
            {
                smallestDistance = distanceToItem;
                closestItem = items[i].gameObject;
            }
        }
    }

    void ShootItem()
    {
        Vector2 shootDirection = cursorPosition - (Vector2)transform.position;
        shootDirection.Normalize();
        ReleaseItem();
        closestItem.GetComponent<Rigidbody2D>().AddForce(shootDirection * shootPower, ForceMode2D.Impulse);
    }

    void SetStableItem(GameObject go, bool b, float t = 0f)
    {
        StableItemHandling sih = go.GetComponent<StableItemHandling>();
        if (sih == null)
        {
            sih = go.AddComponent<StableItemHandling>();
            sih.telekinesis = this;
        }

        if (b)
        {
            stableItems.Add(go);
            sih.SetStable(LayerMask.NameToLayer(groundLayerS));
            sih.SetUnstableAfter(t, LayerMask.NameToLayer(itemsLayerS));
        }
        else
        {
            sih.SetUnstableAfter(t, LayerMask.NameToLayer(itemsLayerS));
        }
    }

    void ReleaseItem()
    {
        closestItem.transform.SetParent(null);
        Rigidbody2D closestItemRigidbody = closestItem.GetComponent<Rigidbody2D>();
        closestItemRigidbody.velocity = Vector2.zero;
        closestItemRigidbody.angularVelocity = 0f;
        closestItemRigidbody.simulated = true;
        state.isHoldingItemState = false;
    }

    public void RemoveStableItem()
    {
        stableItems.RemoveAt(0);
    }
}
