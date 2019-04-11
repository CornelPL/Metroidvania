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
    public bool isKnockbackedState = false;

    [Header("Skills")]

    public bool hasDoubleJump = false;
    public bool hasDash = false;
    public bool hasFlying = false;
    public bool hasExplosion = false;
    public bool hasSlam = false;

    [Header("Helpers")]

    [SerializeField] private PolygonCollider2D normalCollider = null;
    [SerializeField] private PolygonCollider2D invulnerableCollider = null;
    [SerializeField] private Rigidbody2D _rigidbody = null;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }


    private void Update()
    {
        isFallingState = _rigidbody.velocity.y < 0 ? true : false;
    }


    public void EnableInvulnerability()
    {
        normalCollider.enabled = false;
        invulnerableCollider.enabled = true;
    }


    public void DisableInvulnerability()
    {
        normalCollider.enabled = true;
        invulnerableCollider.enabled = false;
    }
}
