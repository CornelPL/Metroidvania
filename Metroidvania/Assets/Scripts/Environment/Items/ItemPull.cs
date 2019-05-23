using UnityEngine;
using UnityEngine.Assertions;

public class ItemPull : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private Collider2D _collider = null;
    [SerializeField] private ItemHandling _itemHandling = null;

    private Transform holdingItemPlace;
    private Vector2 direction;
    private float pullSpeed;
    private float maxPullSpeed;
    private float pullingTime = 0f;
    private float gravityScaleCopy;


    private void Awake()
    {
        gravityScaleCopy = _rigidbody.gravityScale;

        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_collider);
    }


    private void Update()
    {
        pullingTime += Time.deltaTime;

        direction = holdingItemPlace.position - transform.position;
        direction.Normalize();

        float distance = Vector2.Distance(transform.position, holdingItemPlace.position);

        float speed = _rigidbody.velocity.magnitude;

        if (speed < maxPullSpeed)
            speed += pullSpeed * pullingTime;

        _rigidbody.velocity = direction * speed;

        // Check if object is in place
        if (distance < 1f)
        {
            PullingComplete();
        }
    }


    private void PullingComplete()
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.angularVelocity = 0f;
        transform.SetParent(holdingItemPlace);
        transform.position = holdingItemPlace.position;
        _rigidbody.simulated = false;
        PlayerState.instance.isHoldingItemState = true;
        pullingTime = 0f;
        _itemHandling.OnPullingComplete.Invoke();
        StopPulling();
    }


    private void OnDestroy()
    {
        StopPulling();
    }


    public void Pull(Transform t, float s, float ms)
    {
        _itemHandling.OnStartPulling.Invoke();
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _collider.enabled = false;
        PlayerState.instance.isPullingItemState = true;
        holdingItemPlace = t;
        pullSpeed = s;
        maxPullSpeed = ms;
        _rigidbody.gravityScale = 0f;
        if (GetComponent<ItemShoot>().itemType == ItemShoot.ItemType.plank)
            gameObject.layer = LayerMask.NameToLayer("Planks");
    }


    public void StopPulling()
    {
        PlayerState.instance.isPullingItemState = false;
        _rigidbody.gravityScale = gravityScaleCopy;
        _collider.enabled = true;
        gameObject.layer = LayerMask.NameToLayer("Items");
        this.enabled = false;
    }
}