using UnityEngine;

public class DemoBossIdle : StateMachineBehaviour
{
    [SerializeField] private float idleTime = 1.5f;


    private DemoBoss boss;
    private float t = 0f;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        boss = animator.GetComponent<DemoBoss>();
        boss.currentSequence = 0;
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( idleTime > t )
        {
            t += Time.deltaTime;
        }
        else
        {
            ChooseAction( animator );
        }
    }


    private void ChooseAction( Animator animator )
    {
        if ( boss.wasShooting )
        {
            animator.SetBool( "isMoving", true );
            boss.wasShooting = false;
        }
        else
        {
            animator.SetBool( "isShooting", true );
        }
    }
}
