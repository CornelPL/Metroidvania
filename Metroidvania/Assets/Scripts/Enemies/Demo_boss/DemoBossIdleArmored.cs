using UnityEngine;

public class DemoBossIdleArmored : StateMachineBehaviour
{
    [SerializeField] private float idleTime = 1f;


    private DemoBoss boss;
    private float t = 0f;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        t = 0f;
        boss = animator.GetComponent<DemoBoss>();
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
}
