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
        public EnemyHealthManager healthManager;
    }

    [SerializeField] private bool setSavePoint = false;
    [SerializeField, ConditionalField( nameof( setSavePoint ) )] private Transform savePoint = null;
    [SerializeField] protected bool isBlacked = false;
    [SerializeField, ConditionalField( nameof( isBlacked ) )] protected GameObject black = null;
    [SerializeField] private UnityEvent OnExit = null;
    [SerializeField] private GameObject[] adjacentRooms = null;
    [SerializeField] private Transform enemiesGroup = null;

    protected PlayerState playerState = null;
    private bool deactivateLooped = false;
    private float lastPlayerVisit = 0f;
    private List<EnemyInRoom> enemies = new List<EnemyInRoom>();
    protected float timeToRespawnEnemies = 60f;
    protected bool respawnEnemies = true;


    public virtual void OnPlayerEnter( GameObject player )
    {
        if ( playerState.room != null && playerState.room != this )
        {
            playerState.room.UnloadAdjacentRooms( gameObject );
        }
        playerState.room = this;

        if ( setSavePoint )
        {
            playerState.savePoint = savePoint;
            playerState.saveRoom = gameObject;
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

        if ( isBlacked )
        {
            black.SetActive( true );
        }

        if ( enemiesGroup == null ) return;

        foreach ( Transform child in enemiesGroup )
        {
            EnemyInRoom enemy = new EnemyInRoom();
            enemy.transform = child;
            enemy.gameObject = child.gameObject;
            enemy.startPosition = enemy.transform.position;
            enemy.healthManager = child.GetComponent<EnemyHealthManager>();
            enemies.Add( enemy );
        }
    }


    protected virtual void OnEnable()
    {
        if ( respawnEnemies && Time.time - lastPlayerVisit > timeToRespawnEnemies )
        {
            for ( int i = 0; i < enemies.Count; i++ )
            {
                EnemyInRoom enemy = enemies[ i ];

                enemy.transform.position = enemy.startPosition;
                enemy.gameObject.SetActive( true );
                enemy.healthManager.ResetHP();
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
