using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Grounded : MonoBehaviour
{
    private PlayerState state;
    private bool invoked = false;

    [SerializeField] private UnityEvent OnGrounded = null;
    [SerializeField] private UnityEvent OnGroundedOff = null;

    private void Start()
    {
        state = PlayerState.instance;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!invoked)
        {
            OnGrounded.Invoke();
            invoked = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (state.isGroundedState)
        {
            state.isGroundedState = false;
            OnGroundedOff.Invoke();
            invoked = false;
        }
    }
}
