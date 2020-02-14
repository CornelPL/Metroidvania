using UnityEngine;
using MyBox;

[RequireComponent( typeof( PolygonCollider2D ) )]
public class Room : MonoBehaviour
{
    [SerializeField] private bool setSavePoint = false;
    [SerializeField, ConditionalField( nameof( setSavePoint ) )] private Transform savePoint = null;


    private bool isPlayerInRoom = false;


    public virtual void OnPlayerEnter( GameObject player )
    {
        if ( isPlayerInRoom ) return;

        isPlayerInRoom = true;

        PlayerState.instance.room = this;

        if ( setSavePoint )
        {
            PlayerState.instance.savePoint = savePoint;
        }
    }


    public virtual void OnPlayerExit()
    {
        isPlayerInRoom = false;
    }


    public virtual void OnPlayerDeath()
    {
        OnPlayerExit();
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Player" ) )
        {
            OnPlayerEnter( collider.gameObject );
        }
    }


    private void OnTriggerExit2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Player" ) )
        {
            OnPlayerExit();
        }
    }
}
