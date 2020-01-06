using UnityEngine;

public class prezentItem : Item
{
    [SerializeField] private GameObject tree = null;
    [SerializeField] private SpriteRenderer sprite = null;

    public override void Shoot( Vector2 direction, float power )
    {
        tree.SetActive( true );        
        transform.SetParent( null );
        sprite.enabled = false;
    }


    public override void SetFree()
    {
        
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

        CustomDestroy();
    }
}
