using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
    [SerializeField] private Animator animator = null;

    private PlayerState state;
    private bool isRunningSet = false;
    private bool isFallingSet = false;


    private void Start()
    {
        state = PlayerState.instance;
    }


    private void Update()
    {
        if (state.isRunningState && !isRunningSet)
        {
            isRunningSet = true;
            animator.SetBool("isRunning", isRunningSet);
        }
        else if (!state.isRunningState && isRunningSet)
        {
            isRunningSet = false;
            animator.SetBool("isRunning", isRunningSet);
        }

        if (state.isFallingState && !isFallingSet)
        {
            isFallingSet = true;
            animator.SetBool("isFalling", isFallingSet);
        }
        else if (!state.isFallingState && isFallingSet)
        {
            isFallingSet = false;
            animator.SetBool("isFalling", isFallingSet);
        }
    }
}
