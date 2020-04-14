using UnityEngine;

public class DemoManager : MonoBehaviour
{
    [SerializeField] private Transform player = null;
    [SerializeField] private Transform[] saves = null;

    private PlayerState playerState;


    private void Start()
    {
        playerState = PlayerState.instance;
    }


    private void Update()
    {
        if ( Input.GetKey( KeyCode.LeftControl ) && Input.GetKeyDown( KeyCode.Keypad1 ) )
        {
            player.transform.position = saves[ 0 ].position;
            playerState.room.OnPlayerDeath();
        }
        else if ( Input.GetKey( KeyCode.LeftControl ) && Input.GetKeyDown( KeyCode.Keypad2 ) )
        {
            player.transform.position = saves[ 1 ].position;
            playerState.room.OnPlayerDeath();
        }
        else if ( Input.GetKey( KeyCode.LeftControl ) && Input.GetKeyDown( KeyCode.Keypad3 ) )
        {
            player.transform.position = saves[ 2 ].position;
            playerState.room.OnPlayerDeath();
        }
        else if ( Input.GetKey( KeyCode.LeftControl ) && Input.GetKeyDown( KeyCode.Keypad4 ) )
        {
            player.transform.position = saves[ 3 ].position;
            playerState.room.OnPlayerDeath();
        }
    }
}
