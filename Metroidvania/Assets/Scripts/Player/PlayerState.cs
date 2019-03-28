using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance = null;

    [Header("States")]

    public bool isHoldingItemState = false;
    public bool isPullingItemState = false;
    public bool isGroundedState = false;
    public bool isJumpingState = false;
    public bool isFlyingState = false;
    public bool isFallingState = false;
    public bool isSlammingState = false;
    public bool isDashingState = false;

    [Header("Skills")]

    public bool hasDoubleJump = false;
    public bool hasDash = false;
    public bool hasFlying = false;
    public bool hasExplosion = false;
    public bool hasSlam = false;

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
