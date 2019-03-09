using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float pullSpeed = 2f;
    [SerializeField] private float maxPullSpeed = 20f;
    [SerializeField] private float shootPower = 10f;
    [SerializeField] private Transform holdingItemPlace = null;
    [SerializeField] private float slowmoTimeScale = 0.1f;
    public LayerMask itemsLayer;

    private Camera _camera;
    private GameObject closestItem;
    private InputController input;
    private PlayerState state;
    private Vector2 cursorPosition;

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
                SlowModeOn();
            }
            else if (input.lmbUp)
            {
                SlowModeOff();
                ShootItem();
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
        closestItem.transform.SetParent(null);
        Rigidbody2D closestItemRigidbody = closestItem.GetComponent<Rigidbody2D>();
        closestItemRigidbody.velocity = Vector2.zero;
        closestItemRigidbody.angularVelocity = 0f;
        closestItemRigidbody.bodyType = RigidbodyType2D.Dynamic;
        closestItemRigidbody.AddForce(shootDirection * shootPower);
        state.isHoldingItemState = false;
    }

    void SlowModeOn()
    {
        Time.timeScale = slowmoTimeScale;
        Debug.Log(Time.fixedDeltaTime);
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        Debug.Log(Time.fixedDeltaTime);
    }

    void SlowModeOff()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
