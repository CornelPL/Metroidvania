using UnityEngine;

public class DemoBossMove : StateMachineBehaviour
{
    [SerializeField] private bool isDestinationPlayer = false;
    [SerializeField] private float moveDistance = 10f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rayToWallLength = 1.5f;
    [SerializeField] private LayerMask wallsMask = 0;


    private float destination;
    private int direction;
    private DemoBoss boss;
    private Rigidbody2D rigidbody;


    override public void OnStateEnter( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        boss = animator.GetComponent<DemoBoss>();
        rigidbody = boss.GetComponent<Rigidbody2D>();
        boss.SetDirection();
        direction = boss.direction;

        if ( isDestinationPlayer )
        {
            destination = boss.player.transform.position.x;
        }
        else
        {
            destination = boss.transform.position.x + moveDistance * direction;
        }

        boss.isMoving = true;
    }


    override public void OnStateUpdate( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        if ( (direction == 1 && boss.transform.position.x < destination) || (direction == -1 && boss.transform.position.x > destination) )
        {
            rigidbody.velocity = new Vector2( movementSpeed * direction, rigidbody.velocity.y );
            if ( Physics2D.Raycast( (Vector2)boss.transform.position + Vector2.up, Vector2.right * direction, rayToWallLength, wallsMask ) )
            {
                StopMoving( animator );
            }
        }
        else
        {
            StopMoving( animator );
        }
    }


    public override void OnStateExit( Animator animator, AnimatorStateInfo stateInfo, int layerIndex )
    {
        StopMoving( animator );
    }


    private void StopMoving( Animator animator )
    {
        rigidbody.velocity = Vector2.zero;
        animator.SetBool( "isMoving", false );
        boss.isMoving = false;
    }
}
