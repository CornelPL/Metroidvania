using UnityEngine;

public class ChargerWalk : StateMachineBehaviour
{
    [SerializeField] private float movementSpeed = 2f;

    private Charger charger = null;
    private Rigidbody2D rigidbody = null;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( charger == null )
        {
            charger = animator.GetComponent<Charger>();
            rigidbody = animator.GetComponent<Rigidbody2D>();
        }
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( charger.CanSeePlayer() )
        {
            animator.SetBool( "isWalking", false );
        }

        rigidbody.velocity = new Vector2( movementSpeed * charger.direction, rigidbody.velocity.y );
    }


    override public void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        animator.SetBool( "isWalking", false );
    }
}
