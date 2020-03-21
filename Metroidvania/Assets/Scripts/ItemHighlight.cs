using UnityEngine;

public class ItemHighlight : MonoBehaviour
{
    [SerializeField] private Telekinesis telekinesis = null;


    private void Update()
    {
        // TODO:
        if ( EnergyController.instance.energy >= telekinesis.pullFromSurfaceCost )
        {
        //    // miganie paska z energią na biało ile jej zniknie
        //    // pełne cząsteczki
        //}
        //else
        //{
        //    // miganie paska z energią na czerwono
        //    // brak jakiejś części cząsteczek przy podłożu np.
        }
    }
}
