using UnityEngine;

public class OuterSurfaceHighlight : MonoBehaviour
{
    [SerializeField] private Telekinesis telekinesis = null;
    [SerializeField] private GameObject energy = null;
    [SerializeField] private GameObject noEnergy = null;

    private bool isEnergyActive = false;


    private void Update()
    {
        if ( EnergyController.instance.energy > telekinesis.pullFromSurfaceCost )
        {
            if ( !isEnergyActive )
            {
                isEnergyActive = true;
                energy.SetActive( true );
                noEnergy.SetActive( false );
            }
        }
        else if ( isEnergyActive )
        {
            isEnergyActive = false;
            energy.SetActive( false );
            noEnergy.SetActive( true );
        }
    }
}
