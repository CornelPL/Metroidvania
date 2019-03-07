using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;

    private InputController input;
    private PlayerState state;
    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;
    private bool doubleJumped = false;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpSpeed = 20f;

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

        Vector2 direction = new Vector2(horizontalSpeed, verticalSpeed);
        _rigidbody.velocity = direction;

        #endregion
    }
}
