using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class CustomDestroy : MonoBehaviour
{
    [SerializeField] private UnityEvent OnDestroy = null;
    [SerializeField] private GameObject[] destroyEffects = null;
    [SerializeField] private GameObject[] objectsToSpawnOnDestroy = null;
    [SerializeField, MinMaxRange(0, 10)] private RangedInt numObjectsToSpawn = new RangedInt(1, 3);
    [SerializeField] private float spawnedItemVelocityLoss = 4f;
    [SerializeField] private Vector2 spawnRange = Vector2.zero;
    [SerializeField, MinMaxRange( -1080f, 1080f )]
    private RangedFloat angularVelocity = new RangedFloat(-360f, 360f);


    public void Destroy()
    {
        OnDestroy.Invoke();
    }


    public void Destroy( Vector2 hitVelocity, Vector2 hitPosition = default )
    {
        Destroy();

        SpawnDestroyEffects();

        if ( objectsToSpawnOnDestroy.Length == 0 )
        {
            return;
        }

        int quantity = Random.Range( numObjectsToSpawn.Min, numObjectsToSpawn.Max );

        for ( int a = 0; a < quantity; a++ )
        {
            int objectIndex = Random.Range( 0, objectsToSpawnOnDestroy.Length );

            Vector3 tmpSpawnRange = transform.InverseTransformVector( spawnRange );
            tmpSpawnRange.x = Random.Range( -tmpSpawnRange.x, tmpSpawnRange.x );
            tmpSpawnRange.y = Random.Range( -tmpSpawnRange.y, tmpSpawnRange.y );

            Vector3 spawnPosition = hitPosition == Vector2.zero ? transform.position : (Vector3)hitPosition;
            GameObject inst = Instantiate( objectsToSpawnOnDestroy[ objectIndex ], spawnPosition + tmpSpawnRange, transform.rotation );

            Vector3 randomRotation = transform.eulerAngles;
            randomRotation.z = Random.Range( 0f, 360f );
            inst.transform.eulerAngles = randomRotation;

            float velocityVariation = Random.Range( 0.7f, 1.3f );
            inst.GetComponent<Rigidbody2D>().velocity = hitVelocity / spawnedItemVelocityLoss * velocityVariation;
            inst.GetComponent<Rigidbody2D>().angularVelocity = Random.Range( angularVelocity.Min, angularVelocity.Max );
        }
    }


    public void DestroyObject()
    {
        Destroy( gameObject );
    }


    public void SpawnDestroyEffects()
    {
        for ( int i = 0; i < destroyEffects.Length; i++ )
        {
            GameObject inst = Instantiate( destroyEffects[ i ], transform.position, transform.rotation );
        }
    }
}
