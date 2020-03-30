using System.Collections;
using UnityEngine;

public class FightRoom : Room
{
    [SerializeField] private Door[] doors = null;
    [SerializeField] private int enemiesCount = 5;
    [SerializeField] private float loadRoomTime = 2f;

    private bool isRoomPassed = false;
    private int startEnemiesCount;


    public override void OnPlayerEnter( GameObject player )
    {
        if ( !isRoomPassed && playerState.room != this )
        {
            InputController.instance.SetInputActive( false );
            StartCoroutine( EnablePlayerMovement() );

            for ( int i = 0; i < doors.Length; i++ )
            {
                doors[ i ].Close();
            }
        }

        base.OnPlayerEnter( player );
    }


    public override void OnPlayerDeath()
    {
        enemiesCount = startEnemiesCount;

        base.OnPlayerDeath();
    }


    public void RemoveEnemy()
    {
        enemiesCount--;
        if ( enemiesCount == 0 )
        {
            for ( int i = 0; i < doors.Length; i++ )
            {
                doors[ i ].Open();
            }

            isRoomPassed = true;
            respawnEnemies = false;
        }
    }


    protected override void Start()
    {
        base.Start();

        startEnemiesCount = enemiesCount;
        timeToRespawnEnemies = -1f;
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        if ( !isRoomPassed )
        {
            black.SetActive( true );
            isBlacked = true;
            black.GetComponent<AutoColor>().FadeIn();
        }
    }


    private IEnumerator EnablePlayerMovement()
    {
        yield return new WaitForSeconds( loadRoomTime );
        InputController.instance.SetInputActive( true );
    }
}
