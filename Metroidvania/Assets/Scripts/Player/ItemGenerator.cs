using System.Collections.Generic;
using UnityEngine;


public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> items = null;
    [SerializeField] private List<int> itemsSpawnCosts = null;

    [HideInInspector] public GameObject selectedItem = null;
    [HideInInspector] public int itemSpawnCost = 0;

    private InputController input;
    private int numKey = -1;


    public void SelectItem( int id )
    {
        if ( id < items.Count )
        {
            selectedItem = items[ id ];
            itemSpawnCost = itemsSpawnCosts[ id ];
        }
    }


    private void Start()
    {
        input = InputController.instance;
    }


    private void Update()
    {
        numKey = input.numKey;

        if ( numKey >= 0 )
        {
            SelectItem( numKey );
        }
    }
}
