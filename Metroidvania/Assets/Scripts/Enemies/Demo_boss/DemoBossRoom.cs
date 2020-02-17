using System.Collections.Generic;
using UnityEngine;
using MyBox;

[System.Serializable]
public class Enemy
{
    public GameObject prefab;
    public float spawnChance;
    public int maxCount;
}


public class DemoBossRoom : Room
{
    [Separator("Enemies")]
    [SerializeField] private int minEnemiesSpawn = 2;
    [SerializeField] private int maxEnemiesSpawn = 5;
    [SerializeField] private int maxEnemies = 10;
    [SerializeField] private Transform enemiesSpawnPoint = null;
    [SerializeField] private float minShootForce = 2f;
    [SerializeField] private float maxShootForce = 10f;
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 60f;
    [SerializeField] private List<Enemy> enemies = null;

    [Separator("Spikes")]
    [SerializeField] private int minSpikes = 3;
    [SerializeField] private int maxSpikes = 8;
    [SerializeField] private float spikesWidth = 2f;
    [SerializeField] private Transform spawnPoint1 = null;
    [SerializeField] private Transform spawnPoint2 = null;
    [SerializeField] private GameObject spike = null;
    [SerializeField] private DemoBoss boss = null;
    [SerializeField] private GameObject wall = null;


    private List<List<GameObject>> currentEnemies = new List<List<GameObject>>();
    private Vector2 pos;


    public override void OnPlayerEnter( GameObject player )
    {
        base.OnPlayerEnter( player );

        // TODO: spawn boss and wall anim or sth
        boss.gameObject.SetActive( true );
        wall.SetActive( true );

        boss.AssignPlayer( player.transform );
    }


    public override void OnPlayerExit()
    {
        base.OnPlayerExit();

        boss.Restart();

        boss.gameObject.SetActive( false );
        wall.SetActive( false );
    }


    public void Earthquake()
    {
        int count = CountEnemies();
        if ( count < maxEnemies )
        {
            SpawnEnemies( maxEnemies - count );
        }

        SpawnSpikes();
    }
    

    public void ClearRoom()
    {
        for ( int i = 0; i < currentEnemies.Count; i++ )
        {
            for ( int j = 0; j < currentEnemies[ i ].Count; j++ )
            {
                if ( currentEnemies[ i ][ j ] != null )
                {
                    currentEnemies[ i ][ j ].GetComponent<EnemyHealthManager>().TakeDamage( Vector2.zero, 9999 );
                }
            }
        }
    }


    public void SpawnEnemies( int maxCount )
    {
        pos = enemiesSpawnPoint.position;
        int count;
        if ( maxCount < minEnemiesSpawn )
        {
            count = maxCount;
        }
        else if ( maxCount < maxEnemiesSpawn )
        {
            count = Random.Range( minEnemiesSpawn, maxCount );
        }
        else
        {
            count = Random.Range( minEnemiesSpawn, maxEnemiesSpawn );
        }

        for ( int i = 0; i < count; i++ )
        {
            int enemyIndex = ChooseEnemy();

            if ( enemyIndex != -1 )
            {
                GameObject e = Instantiate( enemies[ enemyIndex ].prefab, pos, Quaternion.identity, null );

                currentEnemies[ enemyIndex ].Add( e );

                float angle = Random.Range( minAngle, maxAngle );
                if ( boss.direction == 1 )
                {
                    angle = 180f - angle;
                }
                Vector2 direction = MyMath.Angles.AngleToVector2( angle );
                float force = Random.Range( minShootForce, maxShootForce );
                e.GetComponent<EnemyHealthManager>().isBeingKnockbacked = true;
                e.GetComponent<Rigidbody2D>().AddForce( direction * force, ForceMode2D.Impulse );
            }
        }
    }


    private void Start()
    {
        for ( int i = 0; i < enemies.Count; i++ )
        {
            currentEnemies.Add( new List<GameObject>() );
        }
    }


    private int CountEnemies()
    {
        int count = 0;
        for ( int i = 0; i < currentEnemies.Count; i++ )
        {
            for ( int j = 0; j < currentEnemies[ i ].Count; j++ )
            {
                if ( currentEnemies[ i ][ j ] != null )
                {
                    count++;
                }
                else
                {
                    currentEnemies[ i ].RemoveAt( j );
                    j--;
                }
            }
        }

        return count;
    }


    private void SpawnSpikes()
    {
        int count = Random.Range( minSpikes, maxSpikes );
        Vector2 previousPos = Vector2.zero;

        for ( int i = 0; i < count; i++ )
        {
            Vector2 pos = GeneratePosition();

            if ( i > 0 )
            {
                while ( Vector2.Distance( previousPos, pos ) < spikesWidth )
                {
                    pos = GeneratePosition();
                }
            }

            previousPos = pos;

            Instantiate( spike, pos, Quaternion.identity, null ).GetComponent<FallingSpikes>().StartFalling();
        }
    }


    private Vector2 GeneratePosition()
    {
        return new Vector2( Random.Range( spawnPoint1.position.x, spawnPoint2.position.x ), Random.Range( spawnPoint1.position.y, spawnPoint2.position.y ) );
    }


    private int ChooseEnemy()
    {
        float percent = Random.Range( 0f, 100f );
        float sum = 0;
        int index = 0;

        for ( int i = 0; i < enemies.Count; i++ )
        {
            sum += enemies[ i ].spawnChance;
            if ( percent < sum )
            {
                index = i;
                break;
            }
        }

        if ( enemies[ index ].maxCount > currentEnemies[ index ].Count )
        {
            return index;
        }
        else
        {
            return -1;
        }
    }
}
