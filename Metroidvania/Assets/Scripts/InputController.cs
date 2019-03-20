using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance = null;

    public KeyCode rightKey;
    public KeyCode leftKey;
    public KeyCode jumpKey;
    public KeyCode lmbKey;
    public KeyCode rmbKey;
    public KeyCode dashRightKey;
    public KeyCode dashLeftKey;

    public bool right { get; private set; }
    public bool left { get; private set; }
    public bool jumpDown { get; private set; }
    public bool jumpUp { get; private set; }
    public bool lmbDown { get; private set; }
    public bool lmbUp { get; private set; }
    public bool rmb { get; private set; }
    public bool dashRight { get; private set; }
    public bool dashLeft { get; private set; }

    public Vector2 cursorPosition;

    private Camera _camera;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    private void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        right = Input.GetKey(rightKey);
        left = Input.GetKey(leftKey);
        jumpDown = Input.GetKeyDown(jumpKey);
        jumpUp = Input.GetKeyUp(jumpKey);
        lmbDown = Input.GetKeyDown(lmbKey);
        lmbUp = Input.GetKeyUp(lmbKey);
        rmb = Input.GetKeyDown(rmbKey);
        dashRight = Input.GetKeyDown(dashRightKey);
        dashLeft = Input.GetKeyDown(dashLeftKey);

        cursorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
