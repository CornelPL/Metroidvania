using UnityEngine;

public class Walker_Horizontal : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minDistance = 0.1f;

    private Rigidbody2D _rigidbody;
    private int direction = 1;
    private Vector2 previousPosition;
    private Vector2 currentPosition;
    private bool isBeingKnockbacked = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        previousPosition = transform.position;

        Debug.Log(Vector2.Distance(previousPosition, currentPosition));
        if (Vector2.Distance(previousPosition, currentPosition) < minDistance)
        {
            ChangeDirection();
        }

        if (!isBeingKnockbacked)
        {
            _rigidbody.velocity = new Vector2(speed * direction, _rigidbody.velocity.y);
        }

        currentPosition = transform.position;
    }

    private void ChangeDirection()
    {
        direction = direction > 0 ? -1 : 1;
    }
}
