using UnityEngine;

public class Saw : Item
{
    [SerializeField] private int durability = 5;
    [SerializeField] private Collider2D _colliderWithEnviro = null;


    public override void Shoot( Vector2 direction, float power )
    {
        base.Shoot( direction, power );
        _colliderWithEnviro.enabled = true;
        float angle = MyMath.Angles.Vector2ToAngle( direction );
        transform.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
    }


    public override void StartPulling( Transform t, float s, float ms, Telekinesis tele )
    {
        base.StartPulling( t, s, ms, tele );
        _colliderWithEnviro.enabled = false;
    }


    public override void AbortPulling()
    {
        base.AbortPulling();
        _colliderWithEnviro.enabled = true;
    }


    protected override void OnTriggerEnter2D( Collider2D collider )
    {
        if ( !OnTriggerEnter2DHit( collider ) )
            return;

        onTriggerEnterHitEvent.Invoke();

        durability--;
        if ( durability == 0 ) CustomDestroy();
    }


    private void OnCollisionEnter2D( Collision2D collision )
    {
        if ( !isShooted )
            return;

        gameObject.layer = LayerMask.NameToLayer( "Items" );
        isShooted = false;
        OnHover( false );
        //_collider.isTrigger = false;
        //colliderWithEnviro.enabled = false;
    }
}
