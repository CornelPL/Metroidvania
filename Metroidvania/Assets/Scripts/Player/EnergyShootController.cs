using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class EnergyShootController : MonoBehaviour
{
    [SerializeField, MustBeAssigned] private Transform holdingItemPlace = null;
    [SerializeField, MustBeAssigned] private GameObject energyProjectile = null;
    [SerializeField, MustBeAssigned] private GameObject energyShootEffects = null;
    [SerializeField] private float energyShootPower = 10f;
    [SerializeField] private float energyRecoverySpeed = 2f;
    [SerializeField] private float timeToDisappear = 2f;
    [SerializeField] private AutoScale energyGroup = null;
    [SerializeField] private Image energyFill = null;
    [SerializeField] private GameObject[] energyPopups = null;

    [HideInInspector] public int energy = 5;

    private InputController input;
    private Queue<GameObject> inactiveEnergyProjectiles = new Queue<GameObject>();
    private List<GameObject> activeEnergyProjectiles = new List<GameObject>();
    private bool energyFull = true;
    private float energyFullTimestamp = 0f;
    private bool areEnergyObjectsHidden = true;


    public void ShootEnergy()
    {
        GameObject proj = null;

        for ( int i = 0; i < activeEnergyProjectiles.Count; i++ )
        {
            proj = activeEnergyProjectiles[ i ];
            if ( !proj.activeSelf )
            {
                StartCoroutine( EnqueueEnergyProjectile( proj ) );
                activeEnergyProjectiles.RemoveAt( i-- );
            }
        }

        if ( inactiveEnergyProjectiles.Count == 0 )
        {
            proj = Instantiate( energyProjectile, holdingItemPlace.position, Quaternion.identity, null );
        }
        else
        {
            proj = inactiveEnergyProjectiles.Dequeue();
            proj.transform.position = holdingItemPlace.position;
        }

        activeEnergyProjectiles.Add( proj );

        Vector2 shootDirection = input.cursorPosition - (Vector2)holdingItemPlace.position;

        float angle = Mathf.Atan2( shootDirection.y, shootDirection.x ) * Mathf.Rad2Deg;

        Instantiate( energyShootEffects, holdingItemPlace.position, Quaternion.AngleAxis( angle, Vector3.forward ) );

        proj.GetComponent<EnergyProjectile>().Shoot( shootDirection.normalized, energyShootPower );

        /*energy--;
        energyFill.fillAmount -= 0.2f;
        if ( areEnergyObjectsHidden )
        {
            ShowEnergyObjects();
        }*/

        CustomCursor.Instance?.OnInteraction();
    }


    private void Start()
    {
        input = InputController.instance;
    }


    private void Update()
    {
        if ( energy < 5 )
        {
            if ( energyFull )
            {
                energyFull = false;
            }

            energyFill.fillAmount += energyRecoverySpeed * Time.deltaTime;

            for ( int i = 0; i < 5; i++ )
            {
                if ( energy == i && energyFill.fillAmount >= 0.198f * (i+1) )
                {
                    energy++;
                    energyPopups[ i ].GetComponent<Image>().enabled = true;
                    energyPopups[ i ].GetComponent<AutoScale>().ScaleUp();
                }
            }
        }
        else if ( !energyFull )
        {
            energyFull = true;
            energyFullTimestamp = Time.time;
        }
        else if ( !areEnergyObjectsHidden && Time.time - energyFullTimestamp > timeToDisappear )
        {
            HideEnergyObjects();
        }
    }


    private IEnumerator EnqueueEnergyProjectile( GameObject projectile )
    {
        yield return new WaitForSeconds( 0.1f );
        inactiveEnergyProjectiles.Enqueue( projectile );
    }


    private void HideEnergyObjects()
    {
        energyGroup.ScaleDown();

        areEnergyObjectsHidden = true;
    }


    private void ShowEnergyObjects()
    {
        energyGroup.ScaleUp();

        areEnergyObjectsHidden = false;
    }
}
