using UnityEngine;
using UnityEngine.UI;
using MyBox;

public enum EnergyGain
{
    OnHit,
    OnKill,
    OnKillBoss,
    OnCounterAttack
}

public class EnergyController : MonoBehaviour
{
    public static EnergyController instance = null;

    [SerializeField, MustBeAssigned] private Image container = null;
    [SerializeField] private int capacity = 20;
    [SerializeField] private int energyGainOnHit = 2;
    [SerializeField] private int energyGainOnKill = 5;
    [SerializeField] private int energyGainOnCounterAttack = 5;

    [HideInInspector] public int energy = 0;
    
    [HideInInspector] public bool isContainerFull = false;


    private void Awake()
    {
        if ( instance == null )
        {
            instance = this;
        }
        else if ( instance != this )
        {
            Destroy( this );
        }
    }


    private void Start()
    {
        UpdateContainer();
    }


    public void UpdateContainer()
    {
        container.fillAmount = (float)energy / capacity;
        if ( energy == capacity )
        {
            isContainerFull = true;
        }
        else
        {
            isContainerFull = false;
        }
    }


    public void AddEnergy( EnergyGain energyGain )
    {
        int energyToAdd = 0;
        if ( energyGain == EnergyGain.OnHit )
        {
            energyToAdd = energyGainOnHit;
        }
        else if ( energyGain == EnergyGain.OnKill )
        {
            energyToAdd = energyGainOnKill;
        }else if ( energyGain == EnergyGain.OnCounterAttack )
        {
            energyToAdd = energyGainOnCounterAttack;
        }

        AddEnergy( energyToAdd );
    }


    public void AddEnergy( int energyGain )
    {
        energy += energyGain;
        Mathf.Clamp( energy, 0, capacity );
        UpdateContainer();
    }


    public bool SubEnergy( int energyToSub )
    {
        if ( energyToSub > energy )
        {
            return false;
        }

        energy -= energyToSub;
        UpdateContainer();

        return true;
    }


    public void EmptyContainer()
    {
        energy = 0;
        isContainerFull = false;
        UpdateContainer();
    }
}
