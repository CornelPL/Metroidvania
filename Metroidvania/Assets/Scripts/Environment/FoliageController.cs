using UnityEngine;

public class FoliageController : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private string animName = "idle";


    private void Start()
    {
        animator.Play( animName, -1, Random.Range( 0f, 1f ) );
    }


    private void OnTriggerEnter2D( Collider2D collision )
    {
        animator.SetTrigger( "collision" );
    }
}
