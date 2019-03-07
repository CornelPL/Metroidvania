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

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float slowingAfterDashSpeed = 1f;

    private void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;
    }

    private void Update()
    {
        #region Movement

        if (input.right) horizontalSpeed = movementSpeed;
        else if (input.left) horizontalSpeed = -movementSpeed;
        else horizontalSpeed = 0f;

        #endregion

        #region Jump

        verticalSpeed = _rigidbody.velocity.y;

        if (input.jump)
        {
            if (Grounded.instance.isGrounded)
            {
                verticalSpeed = jumpSpeed;
                state.isJumpingState = true;
                doubleJumped = false;
            }
            else if (state.isJumpingState && state.hasDoubleJump && !doubleJumped)
            {
                verticalSpeed = jumpSpeed;
                doubleJumped = true;
            }
        }

        #endregion

        #region Dash

        if (input.dashRight && state.hasDash && !state.isDashingState)
        {
            _rigidbody.AddForce(Vector2.right * dashSpeed, ForceMode2D.Impulse);
            state.isDashingState = true;
        }
        else if (input.dashLeft && state.hasDash && !state.isDashingState)
        {
            _rigidbody.AddForce(Vector2.left * dashSpeed, ForceMode2D.Impulse);
            state.isDashingState = true;
        }

        #endregion

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
}
