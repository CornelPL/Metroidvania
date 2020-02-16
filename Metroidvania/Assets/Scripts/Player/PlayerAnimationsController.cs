using UnityEngine;
using System.Collections.Generic;

public class PlayerAnimationsController : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private Transform stepEffectSpawnPoint = null;
    [SerializeField] private GameObject[] stepEffectsBase = null;

    private PlayerState state;
    private bool isRunningSet = false;
    private bool isFallingSet = false;
    private Queue<GameObject> stepEffects = new Queue<GameObject>();


    public void SpawnStepEffect()
    {
        GameObject stepEffect = stepEffects.Dequeue();
        stepEffect.transform.position = stepEffectSpawnPoint.position;
        stepEffect.SetActive( true );
        stepEffects.Enqueue( stepEffect );
    }


    private void Start()
    {
        state = PlayerState.instance;

        for ( int i = 0; i < stepEffectsBase.Length; i++ )
        {
            stepEffects.Enqueue( stepEffectsBase[ i ] );
        }
    }


    private void Update()
    {
        if (state.isRunningState && !isRunningSet)
        {
            isRunningSet = true;
            animator.SetBool("isRunning", isRunningSet);
        }
        else if (!state.isRunningState && isRunningSet)
        {
            isRunningSet = false;
            animator.SetBool("isRunning", isRunningSet);
        }

        if (state.isFallingState && !isFallingSet)
        {
            isFallingSet = true;
            animator.SetBool("isFalling", isFallingSet);
        }
        else if (!state.isFallingState && isFallingSet)
        {
            isFallingSet = false;
            animator.SetBool("isFalling", isFallingSet);
        }
    }
}
