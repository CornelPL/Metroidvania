using UnityEngine;
using System.Collections.Generic;

public class SpawnStepEffect : MonoBehaviour
{
    [SerializeField] private Transform stepEffectSpawnPoint = null;
    [SerializeField] private Transform stepEffectSpawnPoint2 = null;
    [SerializeField] private GameObject[] stepEffectsBase = null;

    private Queue<GameObject> stepEffects = new Queue<GameObject>();


    public void SpawnStepEffects()
    {
        GameObject stepEffect = stepEffects.Dequeue();
        stepEffect.transform.position = stepEffectSpawnPoint.position;
        stepEffect.SetActive( true );
        stepEffects.Enqueue( stepEffect );
    }


    public void Spawn2StepEffects()
    {
        SpawnStepEffects();

        GameObject stepEffect = stepEffects.Dequeue();
        stepEffect.transform.position = stepEffectSpawnPoint2.position;
        stepEffect.SetActive( true );
        stepEffects.Enqueue( stepEffect );
    }


    private void Start()
    {
        for ( int i = 0; i < stepEffectsBase.Length; i++ )
        {
            stepEffects.Enqueue( stepEffectsBase[ i ] );
        }
    }
}
