using UnityEngine;
using MyBox;

public class CustomDestroy : MonoBehaviour
{
    [Separator("Destroy Effects")]
    [SerializeField] private GameObject[] destroyEffects = null;
    [SerializeField] private bool rotateWithVelocity = false;
    [SerializeField, ConditionalField( nameof( rotateWithVelocity ) )] private Rigidbody2D _rigidbody = null;

    [Separator("Objects Spawn")]
    [SerializeField] private GameObject[] objectsToSpawnOnDestroy = null;
    [SerializeField, MinMaxRange( 0, 20 )] private RangedInt numObjectsToSpawn = new RangedInt( 1, 3 );
    [SerializeField] private bool inheritVelocity = false;
    [SerializeField, ConditionalField( nameof( inheritVelocity ) )] private float velocityPercentLoss = 80f;
    [SerializeField, ConditionalField( nameof( inheritVelocity ), true )] private float minForce = 10f;
    [SerializeField, ConditionalField( nameof( inheritVelocity ), true )] private float maxForce = 20f;
    [SerializeField, ConditionalField( nameof( inheritVelocity ), true )] private float minAngle = 180f;
    [SerializeField, ConditionalField( nameof( inheritVelocity ), true )] private float maxAngle = 360f;
    [SerializeField] private float minAngularVelocity = 0f;
    [SerializeField] private float maxAngularVelocity = 0f;

    [Separator("Others")]
    [SerializeField] private Vector2 spawnRange = Vector2.zero;


    public void Destroy( Vector2 hitVelocity = default )
    {
        SpawnDestroyEffects();

        if ( objectsToSpawnOnDestroy.Length > 0 )
        {
            SpawnObjects( hitVelocity );
        }

        gameObject.SetActive( false );
    }


    private void SpawnObjects( Vector2 hitVelocity )
    {
        int quantity = Random.Range( numObjectsToSpawn.Min, numObjectsToSpawn.Max );

        for ( int i = 0; i < quantity; i++ )
        {
            int objectIndex = Random.Range( 0, objectsToSpawnOnDestroy.Length );
            
            GameObject inst = Instantiate( objectsToSpawnOnDestroy[ objectIndex ], GetSpawnPosition(), Quaternion.identity );

            Vector3 randomRotation = transform.eulerAngles;
            randomRotation.z = Random.Range( 0f, 360f );
            inst.transform.eulerAngles = randomRotation;

            if ( inheritVelocity )
            {
                float velocityVariation = Random.Range( 0.7f, 1.3f );
                inst.GetComponent<Rigidbody2D>().velocity = hitVelocity * (100f - velocityPercentLoss) / 100f * velocityVariation;
            }
            else
            {
                float force = Random.Range( minForce, maxForce );
                float angle = Random.Range( minAngle, maxAngle );
                Vector2 direction = MyMath.Angles.AngleToVector2( angle );
                direction = transform.TransformDirection( direction );

                inst.GetComponent<Rigidbody2D>().AddForce( direction * force, ForceMode2D.Impulse );
            }

            inst.GetComponent<Rigidbody2D>().angularVelocity = Random.Range( minAngularVelocity, maxAngularVelocity );
        }
    }


    private Vector2 GetSpawnPosition()
    {
        Vector2 spawnPosition = transform.position;

        if ( spawnRange != Vector2.zero )
        {
            Vector2 tmpSpawnRange = spawnRange;
            tmpSpawnRange = transform.InverseTransformVector( tmpSpawnRange );
            tmpSpawnRange.x = Random.Range( -tmpSpawnRange.x, tmpSpawnRange.x );
            tmpSpawnRange.y = Random.Range( -tmpSpawnRange.y, tmpSpawnRange.y );
            spawnPosition += tmpSpawnRange;
        }

        return spawnPosition;
    }


    private void SpawnDestroyEffects()
    {
        for ( int i = 0; i < destroyEffects.Length; i++ )
        {
            Transform effect = Instantiate( destroyEffects[ i ], GetSpawnPosition(), transform.rotation, null ).transform;

            if ( rotateWithVelocity )
            {
                float angle = Mathf.Atan2( _rigidbody.velocity.y, _rigidbody.velocity.x ) * Mathf.Rad2Deg + 90f;
                effect.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
            }
        }
    }
}
