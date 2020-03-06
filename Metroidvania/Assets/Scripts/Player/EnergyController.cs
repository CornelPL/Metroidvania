using UnityEngine;
using UnityEngine.UI;
using MyBox;

public enum EnergyGain
{
    OnHit,
    OnKill,
    OnKillBoss
}

public class EnergyController : MonoBehaviour
{
    public static EnergyController instance = null;

    [SerializeField, MustBeAssigned] private Image container = null;
    [SerializeField] private int capacity = 20;
    [SerializeField] private int energyGainOnHit = 2;
    [SerializeField] private int energyGainOnKill = 5;

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
        Mathf.Clamp( energy, 0, capacity );
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
        if ( energy < capacity )
        {
            int energyToAdd;
            if ( energyGain == EnergyGain.OnHit )
            {
                energyToAdd = energyGainOnHit;
            }
            else if ( energyGain == EnergyGain.OnKill )
            {
                energyToAdd = energyGainOnKill;
            }
            else
            {
                energyToAdd = capacity;
            }

            energy += energyToAdd;
            UpdateContainer();
        }
    }


    public void SubEnergy( int energyToSub )
    {
        if ( energy > 0 )
        {
            energy -= energyToSub;
            UpdateContainer();
        }
    }


    public void EmptyContainer()
    {
        energy = 0;
        isContainerFull = false;
        UpdateContainer();
    }
}
