using UnityEngine;

public class ArmorPart : MonoBehaviour
{
    [SerializeField] private GameObject onFallEffect = null;

    private float lastSpawnTime = 0f;


    private void OnCollisionEnter2D( Collision2D collision )
    {
        if (Time.time - lastSpawnTime > 0.5f )
        {
            float angle = Mathf.Atan2( collision.GetContact( 0 ).normal.y, collision.GetContact( 0 ).normal.x ) * Mathf.Rad2Deg - 90f;

            GameObject instantiated = Instantiate( onFallEffect, collision.GetContact( 0 ).point, Quaternion.Euler( 0f, 0f, angle ) );

            float scale = 1f;

            float mag = collision.relativeVelocity.magnitude;
            if ( mag < 5f )
            {
                scale = mag / 5f;
            }

            instantiated.transform.localScale = new Vector3( scale, scale, 1f );

            lastSpawnTime = Time.time;
        }
    }
}
