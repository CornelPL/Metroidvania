using UnityEngine;

public class DemoBossShoot : StateMachineBehaviour
{
    [SerializeField] private int minProjectiles = 4;
    [SerializeField] private int maxProjectiles = 6;
    [SerializeField] private int sequences = 3;
    [SerializeField] private float clipLength = 8f;
    [SerializeField] private float shootFrame = 3f;


    private DemoBoss boss = null;
    private bool shooted = false;


    public override void OnStateEnter( Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex )
    {
        if ( boss == null )
        {
            boss = animator.GetComponent<DemoBoss>();
        }
        shooted = false;
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( stateInfo.normalizedTime >= shootFrame / clipLength && !shooted )
        {
            shooted = true;

            boss.SetDirection();

            int num = Random.Range( minProjectiles, maxProjectiles );
            for ( int i = 0; i < num; i++ )
            {
                boss.ShootProjectile();
            }

            boss.currentSequence++;

            if ( boss.currentSequence == sequences )
            {
                boss.currentSequence = 0;
                animator.SetBool( "isShooting", false );
                boss.wasShooting = true;
            }
        }
    }
}
