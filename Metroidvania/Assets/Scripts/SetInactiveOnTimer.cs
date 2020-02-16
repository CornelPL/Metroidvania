using UnityEngine;

public class SetInactiveOnTimer : MonoBehaviour
{
    [SerializeField] private float delay = 2f;


    private void OnEnable()
    {
        Invoke( nameof( Deactivate ), delay );
    }


    private void Deactivate()
    {
        gameObject.SetActive( false );
    }
}
