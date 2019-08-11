using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Grounded : MonoBehaviour
{
    private PlayerState state;
    private bool invoked = false;

    [SerializeField] private UnityEventGameObject OnGrounded = null;
    [SerializeField] private UnityEvent OnGroundedOff = null;


    private void Start()
    {
        state = PlayerState.instance;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!invoked)
        {
            OnGrounded.Invoke(collision.gameObject);
            invoked = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collider)
    {
        if (state.isGroundedState)
        {
            OnGroundedOff.Invoke();
            invoked = false;
        }
    }
}
