using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance = null;

    [Header("States")]

    public bool isPullingItemState = false;
    public bool isHoldingItemState = false;
    public bool isGroundedState = false;
    public bool isRunningState = false;
    public bool isJumpingState = false;
    public bool isFallingState = false;
    public bool isSlammingState = false;
    public bool isDashingState = false;
    public bool isKnockbackedState = false;
    public bool isInvulnerable = false;

    [Header("Skills")]

    public bool hasDoubleJump = false;
    public bool hasDash = false;
    public bool hasUpgradedDash = false;
    public bool hasSlam = false;

    [Header("Helpers")]

    [SerializeField] private Collider2D normalCollider = null;
    [SerializeField] private Collider2D invulnerableCollider = null;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private Animator animator = null;
    private float t = 0f;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }


    private void Update()
    {
        isFallingState = _rigidbody.velocity.y < -0.1f ? true : false;
        isRunningState = Mathf.Abs(_rigidbody.velocity.x) > 1f ? true : false;

        if (isInvulnerable)
        {
            if (t > 0f)
                t -= Time.deltaTime;
            else
            {
                normalCollider.enabled = true;
                invulnerableCollider.enabled = false;
                isInvulnerable = false;
            }
        }
    }


    public void EnableInvulnerability()
    {
        normalCollider.enabled = false;
        invulnerableCollider.enabled = true;
        isInvulnerable = true;
        t = 999f;
    }


    public void DisableInvulnerability(float after = 0f)
    {
        t = after;
    }


    public void SetGroundedState(bool b)
    {
        isGroundedState = b;
        animator.SetBool("isGrounded", b);

        if (b)
        {
            isJumpingState = false;
            isFallingState = false;
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
    }
}
