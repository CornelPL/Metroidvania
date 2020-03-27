using UnityEngine;

public class DestroyablePlanks : MonoBehaviour
{
    [SerializeField] private int maxHealth = 6;
    [SerializeField] private CustomDestroy customDestroy = null;

    private int currentHealth;


    public void GetHit( int damage, Vector2 hitVelocity = default, Vector2 hitPosition = default )
    {
        currentHealth -= damage;
        
        if ( currentHealth <= 0 )
        {
            customDestroy.Destroy( hitVelocity, hitPosition );
        }
        else
        {
            // TODO: Hit effects; some shake or sth
        }
    }


    private void Start()
    {
        currentHealth = maxHealth;
    }
}
