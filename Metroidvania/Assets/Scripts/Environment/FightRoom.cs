using UnityEngine;

public class FightRoom : Room
{
    [SerializeField] private Door[] doors = null;
    [SerializeField] private int enemiesCount = 5;


    public override void OnPlayerEnter()
    {
        base.OnPlayerEnter();

        for ( int i = 0; i < doors.Length; i++ )
        {
            doors[ i ].Close();
        }
    }


    public override void OnPlayerExit()
    {
        base.OnPlayerExit();

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
