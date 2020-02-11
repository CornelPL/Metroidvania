using UnityEngine;

public class ChargerCharge : StateMachineBehaviour
{
    [SerializeField] private float chargeSpeed = 15f;

    private Charger charger = null;
    private Rigidbody2D rigidbody = null;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( charger == null )
        {
            charger = animator.GetComponent<Charger>();
            rigidbody = animator.GetComponent<Rigidbody2D>();
        }

        charger.isCharging = true;
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        rigidbody.velocity = new Vector2( chargeSpeed * charger.direction, rigidbody.velocity.y );
    }


    override public void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        charger.isCharging = false;
    }
}
