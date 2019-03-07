using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance = null;

    public KeyCode rightKey;
    public KeyCode leftKey;
    public KeyCode jumpKey;
    public KeyCode lmbKey;
    public KeyCode rmbKey;

    public bool right { get; private set; }
    public bool left { get; private set; }
    public bool jump { get; private set; }
    public bool lmb { get; private set; }
    public bool rmb { get; private set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Update()
    {
        right = Input.GetKey(rightKey);
        left = Input.GetKey(leftKey);
        jump = Input.GetKeyDown(jumpKey);
        lmb = Input.GetKeyDown(lmbKey);
        rmb = Input.GetKeyDown(rmbKey);
    }
}
