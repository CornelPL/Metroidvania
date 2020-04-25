using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float movementSpeedUp = 5f;
    [SerializeField] private float movementSlowDown = 5f;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float jumpSlowTime = 0.2f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float maxFallingSpeed = 50f;
    [SerializeField] private float attackInAirTime = 0.3f;
    [SerializeField] private GameObject[] jumpEffectsBase = null;
    [SerializeField] private PlaySound _jumpSound = null;

    [Header( "Dash" )]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashTime = 0.5f;

    [Space]
    [SerializeField] private SlamSkill slam = null;


    private InputController input;
    private PlayerState state;
    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;
    private float slamSpeed = 50f;
    private bool doubleJumped = false;
    private bool canDashRight = false;
    private bool canDashLeft = false;
    private bool isDashingThroughWall = false;
    private bool isDashingRight = false;
    private bool dashedInAir = false;
    private bool isRunningCoroutine = false;
    private Coroutine coroutine = null;
    private Queue<GameObject> jumpEffects = new Queue<GameObject>();


    private int LeanTweenID = -1;


    public void OnAttackInAir()
    {
        if ( state.isFallingState )
        {
            if ( isRunningCoroutine )
            {
                StopCoroutine( coroutine );
            }
            coroutine = StartCoroutine( OnAttackInAirCoroutine() );
        }
    }

    
    private IEnumerator OnAttackInAirCoroutine()
    {
        isRunningCoroutine = true;
        yield return new WaitForSeconds( attackInAirTime );
        isRunningCoroutine = false;
    }


    public void OnGrounded( GameObject go = null )
    {
        doubleJumped = false;
        dashedInAir = false;

        if ( state.isSlammingState )
        {
            slam.OnSlamEnd( go );
        }
    }


    private void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;

        slamSpeed = slam.slamSpeed;

        for ( int i = 0; i < jumpEffectsBase.Length; i++ )
        {
            jumpEffects.Enqueue( jumpEffectsBase[ i ] );
        }
    }


    private void Update()
    {
        if ( TimeManager.instance.isGamePaused || state.isDeadState )
        {
            return;
        }

        if ( !state.isDashingState && !state.isSlammingState && !state.isKnockbackedState && !state.isHealingState )
        {
            CheckMovement();

            CheckJump();

            CheckSlam();

            CheckDash();
        }

        ApplyMovement();
    }


    private void CheckMovement()
    {
        if ( input.right )
        {
            if ( horizontalSpeed < 0f )
            {
                horizontalSpeed *= -1f;
            }
            else if ( horizontalSpeed < movementSpeed )
            {
                horizontalSpeed += movementSpeedUp * Time.deltaTime;
            }
            else
            {
                horizontalSpeed = movementSpeed;
            }
        }
        else if ( input.left )
        {
            if ( horizontalSpeed > 0f )
            {
                horizontalSpeed *= -1f;
            }
            else if ( horizontalSpeed > -movementSpeed )
            {
                horizontalSpeed -= movementSpeedUp * Time.deltaTime;
            }
            else
            {
                horizontalSpeed = -movementSpeed;
            }
        }
        else
        {
            if ( horizontalSpeed > 0f )
            {
                horizontalSpeed -= movementSlowDown * Time.deltaTime;
                if ( horizontalSpeed < 0f ) horizontalSpeed = 0f;
            }
            else if ( horizontalSpeed < 0f )
            {
                horizontalSpeed += movementSlowDown * Time.deltaTime;
                if ( horizontalSpeed > 0f ) horizontalSpeed = 0f;
            }
        }
    }


    private void CheckJump()
    {
        if ( !(LeanTweenID > 0 && LeanTween.isTweening( LeanTweenID )) )
        {
            if ( isRunningCoroutine )
            {
                verticalSpeed = 0f;
            }
            else
            {
                verticalSpeed = _rigidbody.velocity.y;
            }
        }

        if ( input.jumpDown )
        {
            if ( state.isGroundedState || (Time.time - state.lastTimeGrounded < coyoteTime) )
            {
                state.SetJumpingState();
                SpawnJumpEffect();
                _jumpSound.Play();

                verticalSpeed = jumpSpeed;
            }
            else if ( (state.isJumpingState || state.isFallingState) && state.hasDoubleJump && !doubleJumped )
            {
                state.SetJumpingState();
                SpawnJumpEffect();
                _jumpSound.Play();

                if ( LeanTweenID > 0 && LeanTween.isTweening( LeanTweenID ) )
                {
                    LeanTween.cancel( LeanTweenID );
                    LeanTweenID = -1;
                }
                verticalSpeed = jumpSpeed;
                doubleJumped = true;
            }
        }
        else if ( input.jumpUp && verticalSpeed > 10f )
        {
            LeanTweenID = LeanTween.value( verticalSpeed, 0f, jumpSlowTime )
                .setOnUpdate( ( float v ) => { verticalSpeed = v; } )
                .setOnComplete( () => { LeanTweenID = -1; } ).id;
        }
    }


    private void SpawnJumpEffect()
    {
        GameObject jumpEffect = jumpEffects.Dequeue();
        jumpEffect.SetActive( true );
        jumpEffects.Enqueue( jumpEffect );
    }


    private void CheckSlam()
    {
        if ( input.down && (state.isJumpingState || state.isFallingState) && state.hasSlam )
        {
            slam.OnSlamStart();
        }
    }


    private void CheckDash()
    {
        if ( ((input.dashRight && canDashRight) || (input.dashLeft && canDashLeft)) && state.hasDash )
        {
            isDashingThroughWall = true;
            isDashingRight = input.dashRight ? true : false;
            StartCoroutine( Dash( input.dashRight ? 1 : -1 ) );
        }
        else if ( (input.dashRight || input.dashLeft) && state.hasDash && !dashedInAir )
        {
            isDashingThroughWall = false;
            dashedInAir = state.isGroundedState ? false : true;
            isDashingRight = input.dashRight ? true : false;
            StartCoroutine( Dash( input.dashRight ? 1 : -1 ) );
        }
    }


    IEnumerator Dash( int direction )
    {
        state.isDashingState = true;
        verticalSpeed = 0f;
        float gravityScaleCopy = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0f;
        state.SetInvulnerable( true );
        float t = dashTime;

        // better shrink player
        transform.localScale = new Vector3( 0.3f, 0.3f, 1f );

        while ( state.isDashingState && t > 0f )
        {
            horizontalSpeed = direction * dashSpeed;

            if ( !isDashingThroughWall ) t -= Time.deltaTime;

            yield return null;
        }

        state.isDashingState = false;
        _rigidbody.gravityScale = gravityScaleCopy;
        transform.localScale = Vector3.one;
        state.SetInvulnerable( false, dashTime );
    }


    private void ApplyMovement()
    {
        if ( verticalSpeed < -maxFallingSpeed ) verticalSpeed = -maxFallingSpeed;
        if ( state.isSlammingState ) verticalSpeed = -slamSpeed;
        if ( state.isKnockbackedState ) return;
        _rigidbody.velocity = new Vector2( horizontalSpeed, verticalSpeed );
    }


    private void OnCollisionEnter2D( Collision2D collision )
    {
        if ( collision.collider.CompareTag( "Platform" ) )
        {
            return;
        }

        /*if ( state.isDashingState && state.hasUpgradedDash && collision.collider.CompareTag( "DestroyableWall" ) )
        {
            collision.collider.GetComponent<CustomDestroy>().Destroy();
        }
        state.isDashingState = false;*/
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Trigger" ) )
        {
            if ( collider.GetComponent<DashTrigger>().isRight )
            {
                if ( state.isDashingState )
                {
                    if ( !isDashingRight )
                        isDashingThroughWall = true;
                    else
                        state.isDashingState = false;
                }

                canDashLeft = true;
            }
            else
            {
                if ( state.isDashingState )
                {
                    if ( isDashingRight )
                        isDashingThroughWall = true;
                    else
                        state.isDashingState = false;
                }

                canDashRight = true;
            }
        }
    }


    private void OnTriggerExit2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Trigger" ) )
        {
            canDashLeft = canDashRight = false;
        }
    }
}