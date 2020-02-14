using UnityEngine;

public class FightRoom : Room
{
    [SerializeField] private Door[] doors = null;
    [SerializeField] private int enemiesCount = 5;


    public override void OnPlayerEnter( GameObject player )
    {
        base.OnPlayerEnter( player );

        for ( int i = 0; i < doors.Length; i++ )
        {
            doors[ i ].Close();
        }
    }


    public override void OnPlayerDeath()
    {
        base.OnPlayerDeath();

        for ( int i = 0; i < doors.Length; i++ )
        {
            doors[ i ].Open();
        }
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
        }
    }
}
