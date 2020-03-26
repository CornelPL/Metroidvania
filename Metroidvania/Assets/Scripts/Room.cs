using UnityEngine;
using MyBox;
using UnityEngine.Events;

[RequireComponent( typeof( PolygonCollider2D ) )]
public class Room : MonoBehaviour
{
    [SerializeField] private bool setSavePoint = false;
    [SerializeField, ConditionalField( nameof( setSavePoint ) )] private Transform savePoint = null;
    [SerializeField] private bool isBlacked = false;
    [SerializeField, ConditionalField( nameof( isBlacked ) )] private Transform black = null;
    [SerializeField] private UnityEvent OnExit = null;
    [SerializeField] private GameObject[] adjacentRooms = null;


    private PlayerState playerState = null;
    private bool isPlayerInRoom = false;
    private bool deactivateLooped = false;


    public virtual void OnPlayerEnter( GameObject player )
    {
        if ( isPlayerInRoom ) return;

        isPlayerInRoom = true;

        playerState.room?.UnloadAdjacentRooms( gameObject );

        playerState.room = this;

        if ( setSavePoint )
        {
            playerState.savePoint = savePoint;
        }

        if ( isBlacked )
        {
            black.GetComponent<AutoColor>().FadeOut();
            isBlacked = false;
        }

        if ( !deactivateLooped )
        {
            UnloadAdjacentRooms();
            deactivateLooped = true;
        }

        LoadAdjacentRooms();
    }


    public virtual void OnPlayerExit()
    {
        isPlayerInRoom = false;

        OnExit.Invoke();
    }


    public virtual void OnPlayerDeath()
    {
        OnPlayerExit();

        UnloadAdjacentRooms();

        gameObject.SetActive( false );
    }


    private void Start()
    {
        playerState = PlayerState.instance;
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Player" ) && collider.name != "Shield" )
        {
            OnPlayerEnter( collider.gameObject );
        }
    }


    private void OnTriggerExit2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Player" ) && collider.name != "Shield" )
        {
            OnPlayerExit();
        }
    }


    private void LoadAdjacentRooms()
    {
        for ( int i = 0; i < adjacentRooms.Length; i++ )
        {
            GameObject adjacentRoom = adjacentRooms[ i ];
            if ( !adjacentRoom.activeSelf )
            {
                adjacentRoom.SetActive( true );
            }
        }
    }


    private void UnloadAdjacentRooms( GameObject currentRoom = null )
    {
        if ( !deactivateLooped )
        {
            deactivateLooped = true;
            for ( int i = 0; i < adjacentRooms.Length; i++ )
            {
                GameObject adjacentRoom = adjacentRooms[ i ];

                if ( adjacentRoom.activeSelf && adjacentRoom != currentRoom )
                {
                    adjacentRoom.GetComponent<Room>()?.UnloadAdjacentRooms( playerState.room.gameObject );

                    adjacentRoom.SetActive( false );
                }
            }
        }
        else
        {
            for ( int i = 0; i < adjacentRooms.Length; i++ )
            {
                GameObject adjacentRoom = adjacentRooms[ i ];

                if ( adjacentRoom.activeSelf && adjacentRoom != currentRoom )
                {
                    adjacentRoom.SetActive( false );
                }
            }
        }
    }
}
