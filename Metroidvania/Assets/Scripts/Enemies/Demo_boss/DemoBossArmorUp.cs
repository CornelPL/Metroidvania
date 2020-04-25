using UnityEngine;

public class DemoBossArmorUp : StateMachineBehaviour
{
    [SerializeField] private float armoringTime = 2f;


    private DemoBoss boss = null;
    private float t = 0f;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        t = 0f;

        animator.SetBool( "isShooting", false );

        if ( boss == null )
        {
            boss = animator.GetComponent<DemoBoss>();
        }
        boss.forceField.SetActive( false );

        boss.idleTime = 0f;

        foreach ( ParticleSystem p in boss.armoringParticles )
        {
            p.Play();
        }
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( armoringTime > t )
        {
            t += Time.deltaTime;
        }
        else
        {
            SetArmored( animator );
        }
    }


    private void SetArmored( Animator animator )
    {
        animator.SetBool( "isArmored", true );
        //boss.IncreasePhase();

        foreach ( ParticleSystem p in boss.armoringParticles )
        {
            p.Stop();
        }

        boss.forceField.SetActive( true );
    }
}
