using UnityEngine;

public class DemoBossRage : StateMachineBehaviour
{
    [SerializeField] private int projectilesCount = 30;
    [SerializeField] private float shootInterval = 0.05f;


    private int direction;
    private int currentProjectile = 0;
    private DemoBoss boss;
    private float t = 0f;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        t = 0f;

        boss = animator.GetComponent<DemoBoss>();
        boss.SetDirection();
        direction = boss.direction;

        foreach ( ParticleSystem p in boss.rageParticles )
        {
            p.Play();
        }
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( t < shootInterval )
        {
            t += Time.deltaTime;
        }
        else if ( currentProjectile < projectilesCount )
        {
            boss.OnRageImpulse.GenerateImpulse();

            boss.ShootProjectile();

            currentProjectile++;

            t = 0f;
        }
        else
        {
            foreach ( ParticleSystem p in boss.rageParticles )
            {
                p.Stop();
            }

            animator.SetTrigger( "rageEnd" );
            boss.IncreasePhase();
        }
    }
}
