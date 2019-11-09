using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateItem : Item
{
    private GameObject[] itemsToSpawnOnDestroy = null;
    private int minItemsToSpawn = 1;
    private int maxItemsToSpawn = 3;


    protected override void OnTriggerEnter2D( Collider2D collider )
    {
        if ( !isShooted )
        {
            return;
        }

        if ( collidersToIgnore.Find( ( Collider2D x ) => x == collider ) )
        {
            return;
        }

        base.OnTriggerEnter2D( collider );

        CustomDestroy();
    }


    protected override void CustomDestroy()
    {
        int item = Random.Range( 0, itemsToSpawnOnDestroy.Length );
        int i = Random.Range( minItemsToSpawn, maxItemsToSpawn );
        Vector2 collisionVelocity = _rigidbody.velocity;

        for ( int a = 0; a < i; a++ )
        {
            Vector3 randomRotation = transform.eulerAngles;
            randomRotation.z = Random.Range( 0f, 360f );
            GameObject inst = Instantiate( itemsToSpawnOnDestroy[ item ], transform.position + (Vector3)Random.insideUnitCircle, transform.rotation );
            inst.transform.eulerAngles = randomRotation;
            inst.GetComponent<Rigidbody2D>().velocity = -collisionVelocity;
        }

        base.CustomDestroy();
    }
}
