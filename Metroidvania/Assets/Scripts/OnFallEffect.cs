using UnityEngine;

public class OnFallEffect : MonoBehaviour
{
    [SerializeField] private GameObject effectToSpawn = null;
    [SerializeField] private float minBetweenSpawnTime = 0.5f;
    [SerializeField] private bool scaleWithSpeed = true;
    [SerializeField] private float scale = 1f;
    [SerializeField] private float maxSpeedToMaxScale = 5f;
    [SerializeField] private float minYSpeed = 1f;

    private float lastSpawnTime = 0f;


    private void OnCollisionEnter2D( Collision2D collision )
    {
        if (Time.time - lastSpawnTime > minBetweenSpawnTime )
        {
            if ( collision.relativeVelocity.y < minYSpeed ) return;


            float angle = Mathf.Atan2( collision.GetContact( 0 ).normal.y, collision.GetContact( 0 ).normal.x ) * Mathf.Rad2Deg - 90f;

            GameObject instantiated = Instantiate( effectToSpawn, collision.GetContact( 0 ).point, Quaternion.Euler( 0f, 0f, angle ) );

            if ( scaleWithSpeed )
            {
                float mag = collision.relativeVelocity.magnitude;

                if ( mag < maxSpeedToMaxScale )
                {
                    scale = mag / maxSpeedToMaxScale;
                }
            }

            instantiated.transform.localScale = new Vector3( scale, scale, 1f );

            lastSpawnTime = Time.time;
        }
    }
}
