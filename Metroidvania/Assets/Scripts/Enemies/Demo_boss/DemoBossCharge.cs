using UnityEngine;

public class DemoBossCharge : StateMachineBehaviour
{
    [SerializeField] private float chargeSpeed = 5f;


    private int direction;
    private DemoBoss boss;
    private Rigidbody2D rigidbody;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        boss = animator.GetComponent<DemoBoss>();
        rigidbody = boss.GetComponent<Rigidbody2D>();
        direction = boss.direction;

        boss.isCharging = true;
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        rigidbody.velocity = new Vector2( chargeSpeed * direction, rigidbody.velocity.y );
    }


    public override void OnStateExit( Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex )
    {
        boss.isCharging = false;
    }
}
