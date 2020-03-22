using UnityEngine;

public class OuterSurfaceHighlight : MonoBehaviour
{
    [SerializeField] private Telekinesis telekinesis = null;
    [SerializeField] private GameObject energy = null;
    [SerializeField] private GameObject noEnergy = null;


    private void Update()
    {
        if ( EnergyController.instance.energy > telekinesis.pullFromSurfaceCost )
        {
            if ( !energy.activeSelf )
            {
                energy.SetActive( true );
                noEnergy.SetActive( false );
            }
        }
        else if ( !noEnergy.activeSelf )
        {
            energy.SetActive( false );
            noEnergy.SetActive( true );
        }
    }
}
