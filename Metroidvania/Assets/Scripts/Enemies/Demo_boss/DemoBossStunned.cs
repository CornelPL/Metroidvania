using UnityEngine;

public class DemoBossStunned : StateMachineBehaviour
{
    [SerializeField] private float invulnerableTime = 1f;


    private float t = 0f;
    private bool spawnedEnemies = false;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        spawnedEnemies = false;
        t = 0f;
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( invulnerableTime > t )
        {
            t += Time.deltaTime;
        }
        else
        {
            animator.SetTrigger( "isVulnerable" );
        }

        if ( t > invulnerableTime / 2f && !spawnedEnemies )
        {
            spawnedEnemies = true;
            animator.GetComponent<DemoBoss>().room.SpawnEnemies( 2 );
        }
    }
}
