using UnityEngine;
using MyBox;

public class CustomDestroy : MonoBehaviour
{
    [SerializeField] private GameObject[] destroyEffects = null;
    [SerializeField] private GameObject[] objectsToSpawnOnDestroy = null;
    [SerializeField, MinMaxRange( 0, 20 )] private RangedInt numObjectsToSpawn = new RangedInt( 1, 3 );
    [SerializeField] private bool rotateDestroyEffects = false;
    [SerializeField, ConditionalField( nameof( rotateDestroyEffects ) )] private Rigidbody2D _rigidbody = null;
    [SerializeField] private bool inheritVelocity = false;
    [SerializeField, ConditionalField( nameof( inheritVelocity ) )] private float velocityPercentLoss = 80f;
    [SerializeField, ConditionalField( nameof( inheritVelocity ), true )] private float minForce = 10f;
    [SerializeField, ConditionalField( nameof( inheritVelocity ), true )] private float maxForce = 20f;
    [SerializeField, ConditionalField( nameof( inheritVelocity ), true )] private float minAngle = 180f;
    [SerializeField, ConditionalField( nameof( inheritVelocity ), true )] private float maxAngle = 360f;
    [SerializeField] private Vector2 spawnRange = Vector2.zero;
    [SerializeField] private float minAngularVelocity = 0f;
    [SerializeField] private float maxAngularVelocity = 0f;


    public void Destroy( Vector2 hitVelocity = default, Vector2 hitPosition = default )
    {
        SpawnDestroyEffects();

        if ( objectsToSpawnOnDestroy.Length > 0 )
        {
            SpawnObjects( hitVelocity, hitPosition );
        }

        gameObject.SetActive( false );
    }


    private void SpawnObjects( Vector2 hitVelocity, Vector2 hitPosition )
    {
        int quantity = Random.Range( numObjectsToSpawn.Min, numObjectsToSpawn.Max );

        for ( int i = 0; i < quantity; i++ )
        {
            int objectIndex = Random.Range( 0, objectsToSpawnOnDestroy.Length );

            Vector3 spawnPosition = hitPosition == Vector2.zero ? transform.position : (Vector3)hitPosition;

            if ( spawnRange != Vector2.zero )
            {
                Vector3 tmpSpawnRange = transform.InverseTransformVector( spawnRange );
                tmpSpawnRange.x = Random.Range( -tmpSpawnRange.x, tmpSpawnRange.x );
                tmpSpawnRange.y = Random.Range( -tmpSpawnRange.y, tmpSpawnRange.y );
                spawnPosition += tmpSpawnRange;
            }

            GameObject inst = Instantiate( objectsToSpawnOnDestroy[ objectIndex ], spawnPosition, Quaternion.identity );

            Vector3 randomRotation = transform.eulerAngles;
            randomRotation.z = Random.Range( 0f, 360f );
            inst.transform.eulerAngles = randomRotation;

            if ( inheritVelocity )
            {
                float velocityVariation = Random.Range( 0.7f, 1.3f );
                inst.GetComponent<Rigidbody2D>().velocity = hitVelocity * velocityPercentLoss / 100f * velocityVariation;
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


    private void SpawnDestroyEffects()
    {
        for ( int i = 0; i < destroyEffects.Length; i++ )
        {
            Transform effect = Instantiate( destroyEffects[ i ], transform.position, transform.rotation, null ).transform;

            if ( rotateDestroyEffects )
            {
                float angle = Mathf.Atan2( _rigidbody.velocity.y, _rigidbody.velocity.x ) * Mathf.Rad2Deg;
                effect.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
            }
        }
    }
}
