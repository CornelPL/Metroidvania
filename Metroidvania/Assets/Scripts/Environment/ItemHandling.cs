using UnityEngine;

public class ItemHandling : MonoBehaviour
{
    private bool isBeingPulled = false;
    private Transform holdingItemPlace;
    private Vector2 direction;      
    private Rigidbody2D rb;
    private float pullForce;
    private float maxPullForce;
    private float pullSpeed;
    private float maxPullSpeed;
    private float pullingTime = 0f;
    private LayerMask mask;
    private int damage;
    private float gravityScaleCopy;
    private bool isColliding = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gravityScaleCopy = rb.gravityScale;
    }

    void Update()
    {
        if (isBeingPulled)
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
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

    public void Pull(Transform t, float f, float mf, float s, float ms, LayerMask _mask)
    {
        PlayerState.instance.isPullingItemState = true;
        isBeingPulled = true;
        holdingItemPlace = t;
        pullForce = f;
        maxPullForce = mf;
        pullSpeed = s;
        maxPullSpeed = ms;
        mask = _mask;
        rb.gravityScale = 0f;
    }

    void PullingComplete()
    {
        rb.velocity = Vector2.zero;
        isBeingPulled = false;
        PlayerState.instance.isPullingItemState = false;
        transform.SetParent(holdingItemPlace);
        rb.gravityScale = gravityScaleCopy;
        rb.simulated = false;
        PlayerState.instance.isHoldingItemState = true;
        Destroy(this);
    }

    public void StopPulling()
    {
        isBeingPulled = false;
        PlayerState.instance.isPullingItemState = false;
        rb.gravityScale = gravityScaleCopy;
        Destroy(this);
    }
}
