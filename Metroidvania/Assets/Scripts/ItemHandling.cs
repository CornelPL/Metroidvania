using UnityEngine;

public class ItemHandling : MonoBehaviour
{
    private bool isBeingPulled = false;
    private Transform holdingItemPlace;
    private Vector2 desiredVector;      
    private Rigidbody2D rb;
    private float pullSpeed;
    private float maxPullSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isBeingPulled)
        {
            desiredVector = holdingItemPlace.position - transform.position;
            desiredVector.Normalize();
            if(pullSpeed < maxPullSpeed)
                pullSpeed *= 1.1f;
            rb.velocity = desiredVector * pullSpeed;

            // Check if object is in place
            if (Vector2.Distance(transform.position, holdingItemPlace.position) < 0.5f)
            {
                PullingComplete();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBeingPulled && !collision.gameObject.GetComponent<Rigidbody2D>())
        {
            StopPulling();
        }
    }

    public void Pull(Transform t, float p, float maxp)
    {
        PlayerState.instance.isPullingItemState = true;
        isBeingPulled = true;
        holdingItemPlace = t;
        pullSpeed = p;
        maxPullSpeed = maxp;
    }

    void PullingComplete()
    {
        rb.velocity = Vector2.zero;
        isBeingPulled = false;
        PlayerState.instance.isPullingItemState = false;
        transform.SetParent(holdingItemPlace);
        GetComponent<Rigidbody2D>().simulated = false;
        PlayerState.instance.isHoldingItemState = true;
        Destroy(this);
    }

    void StopPulling()
    {
        isBeingPulled = false;
        PlayerState.instance.isPullingItemState = false;
        Destroy(this);
    }
}
