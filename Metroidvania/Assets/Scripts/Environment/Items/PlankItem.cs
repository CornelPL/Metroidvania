using UnityEngine;

public class PlankItem : Item
{
    private int durability = 3;


    public override void StartPulling( Transform t, float s, float ms, Telekinesis tele )
    {
        base.StartPulling( t, s, ms, tele );

        // TODO: is it needed?
        gameObject.layer = LayerMask.NameToLayer( "Planks" );
    }


    protected override void OnTriggerEnter2D( Collider2D collider )
    {
        if ( !OnTriggerEnter2DSuccess( collider ) )
            return;

        if ( collider.CompareTag( "SoftWall" ) )
        {
            durability--;
            if ( durability == 0 ) CustomDestroy();
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.bodyType = RigidbodyType2D.Static;
            gameObject.layer = LayerMask.NameToLayer( "PlanksGround" );
            transform.rotation = Quaternion.identity;
        }
        else if ( !collider.CompareTag( "Player" ) )
        {
            CustomDestroy();
        }
    }
}
