using UnityEngine;
using MyBox;

public class Lever : MonoBehaviour
{
    [SerializeField, Layer] private int shootItemLayer = 0;
    [SerializeField, MustBeAssigned] private Door door = null;

    private bool opened = false;


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( !opened && collider.gameObject.layer == shootItemLayer )
        {
            door.Open();
            opened = true;
            GetComponent<Animator>().SetTrigger( "Death" );
        }
    }
}
