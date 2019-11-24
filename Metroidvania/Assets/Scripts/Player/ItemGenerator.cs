using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public struct SpawnItem
{
    public GameObject prefab;
    public int spawnCost;
}


public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private List<SpawnItem> items = null;
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
            selectedItem = items[ id ].prefab;
            itemSpawnCost = items[ id ].spawnCost;
        }

        if ( isMenuVisible )
        {
            ShowMenu( false );
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
        isMenuVisible = value;

        if ( value == true )
        {
            timeManager.Pause();
            OnShowMenu.Invoke();
        }
        else
        {
            timeManager.Resume();
            OnHideMenu.Invoke();
        }
    }
}
