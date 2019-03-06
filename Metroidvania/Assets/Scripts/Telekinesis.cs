using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField] private float radius = 1f;
    [SerializeField] private LayerMask itemsLayer;

    private Camera _camera = null;
    private GameObject closestItem = null;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        Vector2 cursorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(cursorPosition, transform.position) < range)
        {
            FindClosestItem(cursorPosition);
        }
    }

    void FindClosestItem(Vector2 position)
    {
        Collider2D[] items = Physics2D.OverlapCircleAll(position, radius, itemsLayer);

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
}
