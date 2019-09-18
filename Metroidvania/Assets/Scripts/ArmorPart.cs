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

            Instantiate( onFallEffect, collision.GetContact( 0 ).point, Quaternion.Euler( 0f, 0f, angle ) );

            lastSpawnTime = Time.time;
        }
    }
}
