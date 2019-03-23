using UnityEngine;

public class GravityProjectile : MonoBehaviour
{
    public LayerMask explosionMask;
    [SerializeField] private float explosionRange = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // enable explosion particles

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRange, explosionMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Enemy"))
            {
                // deal dmg to enemy
            }
            else if (!colliders[i].CompareTag("StableItem"))
            {
                CustomDestroy cd = colliders[i].GetComponent<CustomDestroy>();
                if (cd)
                    cd.Destroy();
                else
                    Destroy(colliders[i].gameObject);
            }
        }

        Destroy(this.gameObject);
    }
}
