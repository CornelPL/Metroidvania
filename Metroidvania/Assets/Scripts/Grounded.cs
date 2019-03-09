using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Grounded : MonoBehaviour
{
    private int groundLayer;
    private PlayerState state;
    private bool invoked = false;

    [SerializeField] private UnityEvent OnGrounded = null;
    [SerializeField] private UnityEvent OnGroundedOff = null;

    private void Start()
    {
        groundLayer = LayerMask.NameToLayer("Ground");
        state = PlayerState.instance;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            if (!invoked)
            {
                OnGrounded.Invoke();
                invoked = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (state.isGroundedState && collider.gameObject.layer == groundLayer)
        {
            state.isGroundedState = false;
            OnGroundedOff.Invoke();
            invoked = false;
        }
    }
}
