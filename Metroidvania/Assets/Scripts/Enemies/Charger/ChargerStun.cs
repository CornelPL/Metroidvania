using UnityEngine;

public class ChargerStun : StateMachineBehaviour
{
    [SerializeField] private float stunTime = 2.5f;

    private Charger charger = null;
    private float t = 0f;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( charger == null )
        {
            charger = animator.GetComponent<Charger>();
        }

        t = 0f;
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( t < stunTime )
        {
            t += Time.deltaTime;
        }
        else
        {
            animator.SetBool( "isStunned", false );
            charger.SetDirection( -charger.direction );
        }
    }
}
