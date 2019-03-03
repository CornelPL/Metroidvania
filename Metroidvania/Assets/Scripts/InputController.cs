using UnityEngine;

public class InputController
{
    public bool right { get; private set; }
    public bool left { get; private set; }
    public bool jump { get; private set; }

    public void OnUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        right = horizontal > 0f;
        left = horizontal < 0f;
        jump = Input.GetButtonDown("Jump");
    }
}
