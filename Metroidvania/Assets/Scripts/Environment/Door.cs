using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private BoxCollider2D openedCollider = null;
    [SerializeField] private BoxCollider2D closedCollider = null;


    public void Open()
    {
        openedCollider.enabled = true;
        closedCollider.enabled = false;
    }


    public void Close()
    {
        openedCollider.enabled = false;
        closedCollider.enabled = true;
    }
}
