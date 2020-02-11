using UnityEngine;

public class ChargerIdle : StateMachineBehaviour
{
    private Charger charger = null;
    private EnemyHealthManager healthManager = null;

    
    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( charger == null )
        {
            charger = animator.GetComponent<Charger>();
            healthManager = animator.GetComponent<EnemyHealthManager>();
        }
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( healthManager.isBeingKnockbacked ) return;

        if ( charger.CanSeePlayer() )
        {
            charger.SetDirection();
            if ( charger.IsPlayerInRange() )
            {
                animator.SetTrigger( "preCharge" );
            }
        }
        else
        {
            animator.SetBool( "isWalking", true );
        }
    }
}
