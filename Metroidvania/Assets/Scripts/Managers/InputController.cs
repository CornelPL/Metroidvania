using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController instance = null;

    [Header("Movement")]
    public KeyCode rightKey;
    public KeyCode leftKey;
    public KeyCode dashKey;

    [Header("Air")]
    public KeyCode jumpKey;
    public KeyCode flyingKey;
    public KeyCode downKey;

    [Header("Mouse")]
    public KeyCode lmbKey;
    public KeyCode rmbKey;

    public bool right { get; private set; }
    public bool left { get; private set; }
    public bool dashRight { get; private set; }
    public bool dashLeft { get; private set; }

    public bool jumpDown { get; private set; }
    public bool jumpUp { get; private set; }
    public bool down { get; private set; }

    public bool lmbDown { get; private set; }
    public bool lmbUp { get; private set; }
    public bool rmb { get; private set; }

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
        dashRight = right && Input.GetKeyDown(dashKey);
        dashLeft = left && Input.GetKeyDown(dashKey);

        jumpDown = Input.GetKeyDown(jumpKey);
        jumpUp = Input.GetKeyUp(jumpKey);
        down = Input.GetKeyDown(downKey);

        lmbDown = Input.GetKeyDown(lmbKey);
        lmbUp = Input.GetKeyUp(lmbKey);
        rmb = Input.GetKeyDown(rmbKey);

        cursorPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
