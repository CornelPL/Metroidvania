using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> items = null;
    [SerializeField] private List<int> itemsSpawnCosts = null;
    [SerializeField] private UnityEvent OnShowMenu = null;
    [SerializeField] private UnityEvent OnHideMenu = null;

    [HideInInspector] public GameObject selectedItem = null;
    [HideInInspector] public int itemSpawnCost = 0;

    private InputController input;
    private TimeManager timeManager;
    private bool isMenuVisible = false;
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
        timeManager = TimeManager.instance;
    }


    private void Update()
    {
        if ( input.spawnMenu )
        {
            if ( isMenuVisible )
            {
                ShowMenu( false );
            }
            else
            {
                ShowMenu( true );
            }
        }

        numKey = input.numKey;

        if ( numKey >= 0 )
        {
            SelectItem( numKey );
        }
    }


    private void ShowMenu( bool value )
    {
        if ( value == true )
        {
            isMenuVisible = true;
            timeManager.Pause();
            OnShowMenu.Invoke();
        }
        else
        {
            isMenuVisible = false;
            timeManager.Resume();
            OnHideMenu.Invoke();
        }
    }
}
