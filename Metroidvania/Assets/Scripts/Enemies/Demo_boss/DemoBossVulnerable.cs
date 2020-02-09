﻿using UnityEngine;

public class DemoBossVulnerable : StateMachineBehaviour
{
    [SerializeField] private float vulnerableTime = 2f;


    private DemoBoss boss;
    private float t = 0f;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        t = 0f;
        boss = animator.GetComponent<DemoBoss>();
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( vulnerableTime > t )
        {
            t += Time.deltaTime;
        }
        else
        {
            animator.SetBool( "isStunned", false );
        }
    }
}
