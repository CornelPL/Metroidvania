using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Grounded : MonoBehaviour
{
    public static Grounded instance = null;

    private int groundLayer;

    [SerializeField] private UnityEvent OnGrounded = null;
    [SerializeField] private UnityEvent OnGroundedOff = null;

    public bool isGrounded { get; private set; }

    private bool invoked = false;

    private void Start()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);

        groundLayer = LayerMask.NameToLayer("Ground");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            isGrounded = true;
            if (!invoked)
            {
                OnGrounded.Invoke();
                invoked = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (isGrounded == true && collider.gameObject.layer == groundLayer)
        {
            isGrounded = false;
            OnGroundedOff.Invoke();
            invoked = false;
        }
    }
}
