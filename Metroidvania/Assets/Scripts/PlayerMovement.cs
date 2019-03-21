using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;

    private InputController input;
    private PlayerState state;
    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;
    private float direction = 1;
    private bool doubleJumped = false;
    private bool dashedInAir = false;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float runningMultiplier = 1.5f;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float flyingSpeed = 2f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float slowingAfterDashSpeed = 1f;

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
        if (state.hasFlying && input.flying && _rigidbody.velocity.y < 0f)
        {
            state.isFlyingState = true;
            verticalSpeed = -flyingSpeed;
        }
        else
        {
            state.isFlyingState = false;
        }
    }

    private void CheckDash()
    {
        if ((input.dashRight || input.dashLeft) && state.hasDash && !state.isDashingState)
        {
            if (!dashedInAir)
                Dash(input.dashRight ? Vector2.right : Vector2.left);

            dashedInAir = state.isGroundedState ? false : true;
        }
    }

    private void Dash(Vector2 direction)
    {
        _rigidbody.AddForce(direction * dashSpeed, ForceMode2D.Impulse);
        state.isDashingState = true;
    }

    private void ApplyMovement()
    {
        direction = Mathf.Sign(_rigidbody.velocity.x);

        if (state.isDashingState)
        {
            horizontalSpeed = _rigidbody.velocity.x - direction * slowingAfterDashSpeed;

            if (Mathf.Abs(_rigidbody.velocity.x) < movementSpeed)
            {
                state.isDashingState = false;
            }
        }

        _rigidbody.velocity = new Vector2(horizontalSpeed, verticalSpeed);
    }

    public void OnGrounded()
    {
        state.isGroundedState = true;
        state.isJumpingState = false;
        dashedInAir = false;
        doubleJumped = false;
    }
}