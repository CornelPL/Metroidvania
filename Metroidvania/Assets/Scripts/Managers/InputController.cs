using UnityEngine;
using MyBox;
using System.Collections.Generic;

public class InputController : MonoBehaviour
{
    public static InputController instance = null;

    [Separator( "Movement" )]
    public KeyCode rightKey;
    public KeyCode leftKey;
    public KeyCode dashKey;

    [Separator( "Air" )]
    public KeyCode jumpKey;
    public KeyCode jumpKey2;
    public KeyCode flyingKey;
    public KeyCode downKey;

    [Separator( "Mouse" )]
    public KeyCode lmbKey;
    public KeyCode rmbKey;

    [Separator( "Other" )]
    public KeyCode interactKey;
    public KeyCode healKey;
    public KeyCode spawnMenuKey;
    public KeyCode spawnItemKey;
    public List<KeyCode> numKeys;

    public bool right { get; private set; }
    public bool left { get; private set; }
    public bool dashRight { get; private set; }
    public bool dashLeft { get; private set; }

    public bool jumpDown { get; private set; }
    public bool jumpUp { get; private set; }
    public bool down { get; private set; }

    public bool lmbDown { get; private set; }
    public bool lmbHold { get; private set; }
    public bool lmbUp { get; private set; }
    public bool rmb { get; private set; }

    public bool interact { get; private set; }

    public bool healDown { get; private set; }
    public bool healUp { get; private set; }

    public bool spawnItem { get; private set; }
    public bool spawnMenu { get; private set; }

    public int numKey { get; private set; }

    public Vector2 cursorPosition;

    private Camera _camera;
    private bool isInputActive = true;


    public void SetInputActive( bool on )
    {
        isInputActive = on;

        if ( !on )
        {
            right = false;
            left = false;
            dashRight = false;
            dashLeft = false;

            jumpDown = false;
            jumpUp = false;
            down = false;

            lmbDown = false;
            lmbHold = false;
            lmbUp = false;
            rmb = false;

            interact = false;

            healDown = false;
            healUp = false;

            spawnItem = false;
            spawnMenu = false;

            numKey = -1;
        }
    }


    private void Awake()
    {
        if ( instance == null )
            instance = this;
        else if ( instance != this )
            Destroy( this );
    }


    private void Start()
    {
        _camera = GameObject.FindGameObjectWithTag( "MainCamera" ).GetComponent<Camera>();
    }


    private void Update()
    {
        if ( !isInputActive ) return;

        right = Input.GetKey( rightKey );
        left = Input.GetKey( leftKey );
        dashRight = right && Input.GetKeyDown( dashKey );
        dashLeft = left && Input.GetKeyDown( dashKey );

        jumpDown = Input.GetKeyDown( jumpKey ) || Input.GetKeyDown( jumpKey2 );
        jumpUp = Input.GetKeyUp( jumpKey ) || Input.GetKeyUp( jumpKey2 );
        down = Input.GetKeyDown( downKey );

        lmbDown = Input.GetKeyDown( lmbKey );
        lmbHold = Input.GetKey( lmbKey );
        lmbUp = Input.GetKeyUp( lmbKey );
        rmb = Input.GetKeyDown( rmbKey );

        interact = Input.GetKeyDown( interactKey );

        healDown = Input.GetKeyDown( healKey );
        healUp = Input.GetKeyUp( healKey );

        spawnItem = Input.GetKeyDown( spawnItemKey );
        spawnMenu = Input.GetKeyDown( spawnMenuKey );

        for ( int i = 0; i < numKeys.Count; i++ )
        {
            if ( Input.GetKeyDown( numKeys[ i ] ) )
            {
                numKey = i;
                break;
            }

            numKey = -1;
        }

        cursorPosition = _camera.ScreenToWorldPoint( Input.mousePosition );
    }
}
