using UnityEngine;
using System.Collections;
using MyBox;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance = null;

    [Header( "States" )]

    public bool isFacingRight = false;
    public bool isPullingItemState = false;
    public bool isHoldingItemState = false;
    public bool isAttackingState = false;
    public bool isGroundedState = false;
    public bool isRunningState = false;
    public bool isJumpingState = false;
    public bool isFallingState = false;
    public bool isSlammingState = false;
    public bool isDashingState = false;
    public bool isKnockbackedState = false;
    public bool isHealingState = false;
    public bool isDeadState = false;

    [Header( "Skills" )]

    public bool hasDoubleJump = false;
    public bool hasDash = false;
    public bool hasUpgradedDash = false;
    public bool hasSlam = false;
    public bool canModifyStableItems = false;
    public bool canSpawnItems = false;

    [Header( "Helpers" )]

    [SerializeField] private Collider2D normalCollider = null;
    [SerializeField] private Collider2D invulnerableCollider = null;
    [SerializeField] private AutoColor shieldColor = null;
    [SerializeField] private Animator animator = null;
    [SerializeField, Layer] private int invulnerableLayer = 0;
    [SerializeField, Layer] private int normalLayer = 0;


    private Vector2 currentPos;
    private Vector2 previousPos;
    [HideInInspector] public float lastTimeGrounded = 0f;
    [HideInInspector] public Transform savePoint = null;
    [HideInInspector] public GameObject saveRoom = null;
    [HideInInspector] public Room room = null;
    [HideInInspector] public Cinemachine.CinemachineVirtualCamera currentVirtualCamera = null;


    private void Awake()
    {
        if ( instance == null )
            instance = this;
        else if ( instance != this )
            Destroy( this );
    }


    private void Start()
    {
        previousPos = currentPos = transform.position;
    }


    private void Update()
    {
        currentPos = transform.position;
        Vector2 velocity = (currentPos - previousPos) * 10f;
        previousPos = currentPos;

        isFallingState = velocity.y < -0.1f ? true : false;
        isRunningState = Mathf.Abs( velocity.x ) > 0.05f ? true : false;

        if ( velocity.x > 0.1f && !isFacingRight )
        {
            RotatePlayer( true );
        }
        else if ( velocity.x < -0.1f && isFacingRight )
        {
            RotatePlayer( false );
        }
    }


    private void RotatePlayer( bool isRight )
    {
        isFacingRight = isRight;
        animator.SetBool( "isFacingRight", isRight );
    }


    public void SetInvulnerable( bool b, float after = 0f )
    {
        StartCoroutine( SetInvulnerability( b, after ) );
    }


    private IEnumerator SetInvulnerability( bool b, float after = 0f )
    {
        yield return new WaitForSeconds( after );
        if ( b )
        {
            shieldColor.FadeIn();
        }
        else
        {
            shieldColor.FadeOut();
            yield return new WaitForSeconds( shieldColor.fadeOutTime );
        }

        normalCollider.gameObject.layer = b ? invulnerableLayer : normalLayer;
        //normalCollider.enabled = !b;
        invulnerableCollider.enabled =  b;
    }


    public void SetGroundedState( bool b )
    {
        isGroundedState = b;
        lastTimeGrounded = Time.time;
        animator.SetBool( "isGrounded", b );

        if ( b )
        {
            isJumpingState = false;
            isFallingState = false;
            animator.SetBool( "isJumping", false );
            animator.SetBool( "isFalling", false );
        }
    }


    public void SetJumpingState()
    {
        isJumpingState = true;
        animator.SetBool( "isJumping", true );
    }
}
