using UnityEngine;

public class ItemPull : MonoBehaviour
{
    private Transform holdingItemPlace;
    private Vector2 direction;
    private Rigidbody2D rb;
    private float pullForce;
    private float maxPullForce;
    private float pullSpeed;
    private float maxPullSpeed;
    private float pullingTime = 0f;
    private float gravityScaleCopy;
    private bool isColliding = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityScaleCopy = rb.gravityScale;
    }


    private void Update()
    {
        pullingTime += Time.deltaTime;

        direction = holdingItemPlace.position - transform.position;
        direction.Normalize();

        float distance = Vector2.Distance(transform.position, holdingItemPlace.position);

        if (isColliding)
        {
            rb.AddForce(direction * pullForce, ForceMode2D.Force);
        }
        else
        {
            float speed = rb.velocity.magnitude;

            if (speed < maxPullSpeed)
                speed += pullSpeed * pullingTime;

            rb.velocity = direction * speed;
        }

        // Check if object is in place
        if (distance < 1f)
        {
            PullingComplete();
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        isColliding = true;
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        isColliding = false;
    }


    private void PullingComplete()
    {
        rb.velocity = Vector2.zero;
        transform.SetParent(holdingItemPlace);
        transform.position = holdingItemPlace.position;
        rb.simulated = false;
        PlayerState.instance.isHoldingItemState = true;
        StopPulling();
    }


    private void OnDestroy()
    {
        StopPulling();
    }


    public void Pull(Transform t, float f, float mf, float s, float ms)
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        PlayerState.instance.isPullingItemState = true;
        holdingItemPlace = t;
        pullForce = f;
        maxPullForce = mf;
        pullSpeed = s;
        maxPullSpeed = ms;
        rb.gravityScale = 0f;
        if (GetComponent<ItemShoot>().itemType == ItemShoot.ItemType.plank)
            gameObject.layer = LayerMask.NameToLayer("Planks");
        else if (gameObject.layer != LayerMask.NameToLayer("IgnoreCollisions"))
            gameObject.layer = LayerMask.NameToLayer("PullingItem");
    }


    public void StopPulling()
    {
        PlayerState.instance.isPullingItemState = false;
        rb.gravityScale = gravityScaleCopy;
        gameObject.layer = LayerMask.NameToLayer("Items");
        Destroy(this);
    }
}