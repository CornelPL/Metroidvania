using UnityEngine;

public class DemoBossStunned : StateMachineBehaviour
{
    [SerializeField] private float stunTime = 1f;


    private DemoBoss boss;
    private float t = 0f;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        t = 0f;
        boss = animator.GetComponent<DemoBoss>();
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( stunTime > t )
        {
            t += Time.deltaTime;
        }
        else
        {
            animator.SetBool( "isStunned", false );
        }
    }
}
