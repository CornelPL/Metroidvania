using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
    private PlayerState state;
    private Animator animator;


    private void Start()
    {
        state = PlayerState.instance;
    }


    private void Update()
    {
        animator.SetBool("isRunning", state.isRunningState);
        animator.SetBool("isGrounded", state.isGroundedState);
        animator.SetBool("isJumping", state.isJumpingState);
        animator.SetBool("isFalling", state.isFallingState);
    }
}
