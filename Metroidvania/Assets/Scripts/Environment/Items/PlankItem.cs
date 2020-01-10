using UnityEngine;

public class PlankItem : Item
{
    private int durability = 3;


    public override void StartPulling( Transform t, float s, float ms )
    {
        base.StartPulling( t, s, ms );

        gameObject.layer = LayerMask.NameToLayer( "Planks" );
    }


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

        GameObject go = collider.gameObject;

        if ( go.CompareTag( "SoftWall" ) )
        {
            durability--;
            if ( durability == 0 ) CustomDestroy();
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.bodyType = RigidbodyType2D.Static;
            gameObject.layer = LayerMask.NameToLayer( "PlanksGround" );
            transform.rotation = Quaternion.identity;
        }
        else if ( !go.CompareTag( "Player" ) )
        {
            CustomDestroy();
        }
    }
}
