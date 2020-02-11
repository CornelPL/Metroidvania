using UnityEngine;

public class DemoBossIdleArmored : StateMachineBehaviour
{
    [SerializeField] private float idleTime = 1f;


    private DemoBoss boss = null;
    private float t = 0f;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( boss == null )
        {
            boss = animator.GetComponent<DemoBoss>();
        }

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
        }
        else
        {
            boss.SetDirection();
            animator.SetTrigger( "charge" );
        }
    }


    public override void OnStateExit( Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex )
    {
        boss.idleTime = t;
    }
}
