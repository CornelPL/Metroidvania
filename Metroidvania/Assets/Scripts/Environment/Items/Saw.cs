using UnityEngine;

public class Saw : Item
{
    [SerializeField] private int durability = 5;
    [SerializeField] private Collider2D colliderWithEnviro = null;


    public override void Shoot( Vector2 direction, float power )
    {
        base.Shoot( direction, power );
        colliderWithEnviro.enabled = true;
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
        if ( collision.collider == _collider )
            return;

        gameObject.layer = LayerMask.NameToLayer( "Items" );
        isShooted = false;
        _collider.isTrigger = false;
        colliderWithEnviro.enabled = false;
    }
}
