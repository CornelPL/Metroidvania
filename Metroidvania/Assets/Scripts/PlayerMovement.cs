using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;

    private InputController input;
    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpSpeed = 20f;

    private void Start()
    {
        input = InputController.instance;
    }

    private void Update()
    {
        if (input.right) horizontalSpeed = movementSpeed;
        else if (input.left) horizontalSpeed = -movementSpeed;
        else horizontalSpeed = 0f;

        verticalSpeed = _rigidbody.velocity.y;

        if (Grounded.instance.isGrounded && input.jump)
        {
            verticalSpeed += jumpSpeed;
        }

        Vector2 direction = new Vector2(horizontalSpeed, verticalSpeed);
        _rigidbody.velocity = direction;
    }
}
