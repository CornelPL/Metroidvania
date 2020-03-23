using UnityEngine;

public class InnerSurfaceHighlight : MonoBehaviour
{
    [SerializeField] private Telekinesis telekinesis = null;
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private GameObject energy = null;
    [SerializeField] private Color hasEnergyColor = Color.white;
    [SerializeField] private Color noEnergyColor = Color.black;

    private bool isEnergyActive = false;


    private void Update()
    {
        if ( EnergyController.instance.energy > telekinesis.pullFromSurfaceCost )
        {
            if ( !isEnergyActive )
            {
                isEnergyActive = true;
                spriteRenderer.color = hasEnergyColor;
                energy.SetActive( true );
            }
        }
        else if ( isEnergyActive )
        {
            isEnergyActive = false;
            spriteRenderer.color = noEnergyColor;
            energy.SetActive( false );
        }
    }
}
