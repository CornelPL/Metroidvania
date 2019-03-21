using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance = null;

    // States

    [HideInInspector] public bool isHoldingItemState = false;
    [HideInInspector] public bool isPullingItemState = false;
    [HideInInspector] public bool isGroundedState = false;
    [HideInInspector] public bool isJumpingState = false;
    [HideInInspector] public bool isFlyingState = false;
    [HideInInspector] public bool isFallingState = false;
    [HideInInspector] public bool isDashingState = false;
    [HideInInspector] public bool isRunningState = false;

    // Skills

    public bool hasDoubleJump = false;
    public bool hasDash = false;
    public bool hasFlying = false;
    public bool hasExplosion = false;

    // Helpers
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        isFallingState = _rigidbody.velocity.y < 0 ? true : false;
    }
}
