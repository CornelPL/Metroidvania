using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

[RequireComponent( typeof( PolygonCollider2D ) )]
public class Room : MonoBehaviour
{
    protected class EnemyInRoom
    {
        public GameObject gameObject;
        public Transform transform;
        public Vector2 startPosition;
    }

    [SerializeField] private bool setSavePoint = false;
    [SerializeField, ConditionalField( nameof( setSavePoint ) )] private Transform savePoint = null;
    [SerializeField] private bool isBlacked = false;
    [SerializeField, ConditionalField( nameof( isBlacked ) )] private Transform black = null;
    [SerializeField] private UnityEvent OnExit = null;
    [SerializeField] private GameObject[] adjacentRooms = null;
    [SerializeField] private Transform enemiesGroup = null;

    private PlayerState playerState = null;
    private bool isPlayerInRoom = false;
    private bool deactivateLooped = false;
    private float lastPlayerVisit = 0f;
    private List<EnemyInRoom> enemies = new List<EnemyInRoom>();
    private const float timeToRespawnEnemies = 5f;


    public virtual void OnPlayerEnter( GameObject player )
    {
        if ( isPlayerInRoom ) return;
        isPlayerInRoom = true;

        if ( playerState.room != null && playerState.room != this )
        {
            playerState.room.UnloadAdjacentRooms( gameObject );
        }
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


    protected virtual void Start()
    {
        playerState = PlayerState.instance;

        if ( enemiesGroup == null ) return;

        Transform[] tmp = enemiesGroup.GetComponentsInChildren<Transform>();
        for ( int i = 0; i < tmp.Length; i++ )
        {
            EnemyInRoom enemy = new EnemyInRoom();
            Transform child = tmp[ i ];
            enemy.transform = child;
            enemy.gameObject = child.gameObject;
            enemy.startPosition = enemy.transform.position;
            enemies.Add( enemy );
        }
    }


    private void OnEnable()
    {
        if ( Time.time - lastPlayerVisit > timeToRespawnEnemies )
        {
            for ( int i = 0; i < enemies.Count; i++ )
            {
                EnemyInRoom enemy = enemies[ i ];

                enemy.transform.position = enemy.startPosition;
                enemy.gameObject.SetActive( true );
            }
        }
    }


    private void OnDisable()
    {
        lastPlayerVisit = Time.time;
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
