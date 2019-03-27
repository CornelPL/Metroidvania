using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;

    private InputController input;
    private PlayerState state;
    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;
    private bool doubleJumped = false;
    private bool canDashRight = false;
    private bool canDashLeft = false;
    private bool isDashingThroughWall = false;
    private bool dashedInAir = false;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float runningMultiplier = 1.5f;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float flyingSpeed = 2f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashTime = 0.5f;

    private int id = -1;

    private void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;
    }

    private void Update()
    {
        CheckMovement();

        CheckFlying();

        CheckJump();

        CheckDash();

        ApplyMovement();
    }

    private void CheckMovement()
    {
        if (state.isDashingState) return;

        if (input.right) horizontalSpeed = movementSpeed;
        else if (input.left) horizontalSpeed = -movementSpeed;
        else horizontalSpeed = 0f;

        if (input.running && state.isGroundedState)
        {
            horizontalSpeed *= runningMultiplier;
            state.isRunningState = true;
        }
        else
        {
            state.isRunningState = false;
        }
    }

    private void CheckJump()
    {
        if (state.isDashingState) return;

        if (!(id > 0 && LeanTween.isTweening(id)) && !state.isFlyingState)
        {
            verticalSpeed = _rigidbody.velocity.y;
        }

        if (input.jumpDown)
        {
            if (state.isGroundedState)
            {
                verticalSpeed = jumpSpeed;
                state.isJumpingState = true;
            }
            else if ((state.isJumpingState || state.isFallingState) && state.hasDoubleJump && !doubleJumped)
            {
                if (id > 0 && LeanTween.isTweening(id))
                {
                    LeanTween.cancel(id);
                    id = -1;
                }
                state.isJumpingState = true;
                verticalSpeed = jumpSpeed;
                doubleJumped = true;
            }
        }
        else if (input.jumpUp && verticalSpeed > 0f)
        {
            id = LeanTween.value(verticalSpeed, 0f, 0.1f)
                .setOnUpdate((float v) => { verticalSpeed = v; })
                .setOnComplete(() => { id = -1; }).id;
        }
    }

    private void CheckFlying()
    {
        if (state.isDashingState) return;

        if (state.hasFlying && input.flying && _rigidbody.velocity.y < 0f)
        {
            state.isFlyingState = true;
            verticalSpeed = -flyingSpeed;
        }
        
        if (input.flyingUp)
        {
            state.isFlyingState = false;
        }
    }

    private void CheckDash()
    {
        if (((input.dashRight && canDashRight) || (input.dashLeft && canDashLeft)) &&
            state.hasDash &&
            !state.isDashingState)
        {
            isDashingThroughWall = true;
            StartCoroutine(Dash(input.dashRight ? 1 : -1));
        }
        else if ((input.dashRight || input.dashLeft) && state.hasDash && !state.isDashingState && !dashedInAir)
        {
            dashedInAir = state.isGroundedState ? false : true;
            isDashingThroughWall = false;
            StartCoroutine(Dash(input.dashRight ? 1 : -1));
        }
    }

    IEnumerator Dash(int direction)
    {
        state.isDashingState = true;

        // better shrink player
        transform.localScale = new Vector2(0.3f, 0.3f);

        float gravityScaleCopy = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0f;
        verticalSpeed = 0f;

        float t = dashTime;

        while (state.isDashingState && t > 0f)
        {
            horizontalSpeed = direction * dashSpeed;

            if (!isDashingThroughWall) t -= Time.deltaTime;

            yield return null;
        }

        state.isDashingState = false;
        _rigidbody.gravityScale = gravityScaleCopy;
        transform.localScale = new Vector2(1f, 1f);
    }

    private void ApplyMovement()
    {
        _rigidbody.velocity = new Vector2(horizontalSpeed, verticalSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trigger"))
        {
            if (collision.GetComponent<DashTrigger>().direction > 0)
            {
                canDashLeft = true;
            }
            else
            {
                canDashRight = true;
            }

            if (state.isDashingState) state.isDashingState = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Trigger"))
        {
            canDashLeft = canDashRight = false;
        }
    }

    public void OnGrounded()
    {
        state.isGroundedState = true;
        state.isJumpingState = false;
        doubleJumped = false;
        dashedInAir = false;
    }
}