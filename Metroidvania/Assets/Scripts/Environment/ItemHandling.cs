using UnityEngine;

public class ItemHandling : MonoBehaviour
{
    private bool isBeingPulled = false;
    private Transform holdingItemPlace;
    private Vector2 direction;      
    private Rigidbody2D rb;
    private float pullForce;
    private LayerMask mask;
    private int damage;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isBeingPulled)
        {
            direction = holdingItemPlace.position - transform.position;
            direction.Normalize();

            rb.AddForce(direction * pullForce, ForceMode2D.Force);

            // Check if object is in place
            if (Vector2.Distance(transform.position, holdingItemPlace.position) < 0.5f)
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

    public void Pull(Transform t, float p, LayerMask _mask)
    {
        PlayerState.instance.isPullingItemState = true;
        isBeingPulled = true;
        holdingItemPlace = t;
        pullForce = p;
        mask = _mask;
    }

    void PullingComplete()
    {
        rb.velocity = Vector2.zero;
        isBeingPulled = false;
        PlayerState.instance.isPullingItemState = false;
        transform.SetParent(holdingItemPlace);
        rb.simulated = false;
        PlayerState.instance.isHoldingItemState = true;
        Destroy(this);
    }

    public void StopPulling()
    {
        isBeingPulled = false;
        PlayerState.instance.isPullingItemState = false;
        Destroy(this);
    }
}
