using UnityEngine;

public class DemoBossIdle : StateMachineBehaviour
{
    [SerializeField] private float idleTime = 1.5f;


    private DemoBoss boss = null;
    private float t = 0f;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( boss == null )
        {
            boss = animator.GetComponent<DemoBoss>();
        }
        boss.currentSequence = 0;

        boss.SetDirection();

        t = boss.idleTime;

        if ( t > idleTime )
        {
            t = 0f;
        }
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( idleTime > t )
        {
            t += Time.deltaTime;
            boss.SetDirection();
        }
        else
        {
            ChooseAction( animator );
        }
    }


    public override void OnStateExit( Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex )
    {
        // Fixes boss isn't set beacuse OnStateEnter doesn't run on start
        if ( boss == null )
        {
            boss = animator.GetComponent<DemoBoss>();
        }

        boss.idleTime = t;
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
