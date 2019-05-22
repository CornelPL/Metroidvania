using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Assertions;

public class ItemPull : MonoBehaviour
{
    [SerializeField] private UnityEvent OnStartPulling = null;
    [SerializeField] private UnityEvent OnStopPulling = null;
    [SerializeField] private UnityEvent OnPullingComplete = null;

    private Transform holdingItemPlace;
    private Vector2 direction;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private float pullSpeed;
    private float maxPullSpeed;
    private float pullingTime = 0f;
    private float gravityScaleCopy;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
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
        transform.SetParent(holdingItemPlace);
        transform.position = holdingItemPlace.position;
        _rigidbody.simulated = false;
        PlayerState.instance.isHoldingItemState = true;
        pullingTime = 0f;
        OnPullingComplete.Invoke();
        StopPulling();
    }


    private void OnDestroy()
    {
        StopPulling();
    }


    public void Pull(Transform t, float s, float ms)
    {
        OnStartPulling.Invoke();
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
        OnStopPulling.Invoke();
        PlayerState.instance.isPullingItemState = false;
        _rigidbody.gravityScale = gravityScaleCopy;
        _collider.enabled = true;
        gameObject.layer = LayerMask.NameToLayer("Items");
        this.enabled = false;
    }
}