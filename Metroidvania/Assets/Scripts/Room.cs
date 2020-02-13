using UnityEngine;
using MyBox;

[RequireComponent( typeof( PolygonCollider2D ) )]
public class Room : MonoBehaviour
{
    [SerializeField] private bool setSavePoint = false;
    [SerializeField, ConditionalField( nameof( setSavePoint ) )] private Transform savePoint = null;


    private bool isPlayerInRoom = false;


    public virtual void OnPlayerEnter()
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


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Player" ) )
        {
            OnPlayerEnter();
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
