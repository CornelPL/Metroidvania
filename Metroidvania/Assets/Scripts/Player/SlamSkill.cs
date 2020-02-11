using UnityEngine;

public class SlamSkill : MonoBehaviour
{
    private PlayerState state;

    public float slamSpeed = 50f;
    [SerializeField] private float slamRange = 5f;
    [SerializeField] private float itemsKnockbackForce = 15f;
    [SerializeField] private float enemiesKnockbackForce = 8000f;
    [SerializeField] private Vector2Event earthquakeEvent = null;
    [Tooltip( "What should be affected on slam end" )]
    public LayerMask slamMask;


    private void Start()
    {
        state = PlayerState.instance;
    }


    public void OnSlamStart()
    {
        state.isSlammingState = true;
        state.SetInvulnerable( true );
    }


    public void OnSlamEnd( GameObject go )
    {
        state.isSlammingState = false;
        Slam();
        if ( go.CompareTag( "DestroyableGround" ) )
        {
            go.GetComponent<CustomDestroy>().Destroy();
        }
    }


    private void Slam()
    {
        state.SetInvulnerable( false, 0.5f );
        earthquakeEvent.Broadcast( gameObject, transform.position );

        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll( transform.position, slamRange, slamMask );

        for ( int i = 0; i < objectsInRange.Length; i++ )
        {
            Collider2D currentObject = objectsInRange[ i ];

            Vector2 direction = currentObject.transform.position - new Vector3( transform.position.x, transform.position.y - 1f );
            direction.Normalize();
            if ( currentObject.CompareTag( "Item" ) )
            {
                currentObject.attachedRigidbody.AddForce( direction * itemsKnockbackForce, ForceMode2D.Impulse );
            }
            else if ( objectsInRange[ i ].CompareTag( "Enemy" ) )
            {
                currentObject.GetComponent<EnemyHealthManager>().Knockback( direction, enemiesKnockbackForce );
            }
        }
    }
}
