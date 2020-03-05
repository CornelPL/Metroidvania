using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [SerializeField] private int health = 3;
    [SerializeField] private int minPointsToSpawn = 3;
    [SerializeField] private int maxPointsToSpawn = 8;
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 10f;
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 180f;
    [SerializeField] private GameObject point = null;
    [SerializeField] private GameObject[] getHitEffects = null;
    [SerializeField] private GameObject[] destroyEffects = null;
    [SerializeField] private Sprite[] sprites = null;
    [SerializeField] private SpriteRenderer spriteRenderer = null;


    public void GetHit()
    {
        health--;

        SpawnGetHitEffects();

        SpawnPoints();

        if ( health <= 0 )
        {
            SpawnDestroyEffects();

            gameObject.SetActive( false );
        }
        else
        {
            //spriteRenderer.sprite = sprites[ health - 1 ];
        }
    }


    private void SpawnGetHitEffects()
    {
        for ( int i = 0; i < getHitEffects.Length; i++ )
        {
            GameObject inst = Instantiate( getHitEffects[ i ], transform.position, transform.rotation, null );
        }
    }


    private void SpawnDestroyEffects()
    {
        for ( int i = 0; i < destroyEffects.Length; i++ )
        {
            GameObject inst = Instantiate( destroyEffects[ i ], transform.position, transform.rotation, null );
        }
    }


    private void SpawnPoints()
    {
        int quantity = Random.Range( minPointsToSpawn, maxPointsToSpawn );

        if ( health == 0 ) quantity = Mathf.CeilToInt( maxPointsToSpawn * 1.5f );

        for ( int i = 0; i < quantity; i++ )
        {
            GameObject inst = Instantiate( point, transform.position, Quaternion.identity );

            float force = Random.Range( minForce, maxForce );
            float angle = Random.Range( minAngle, maxAngle );
            Vector2 direction = MyMath.Angles.AngleToVector2( angle );
            direction = transform.TransformDirection( direction );

            inst.GetComponent<Rigidbody2D>().AddForce( direction * force, ForceMode2D.Impulse );
        }
    }
}
