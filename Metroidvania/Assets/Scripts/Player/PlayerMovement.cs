using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private Animator animator = null;

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

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float movementSpeedUp = 5f;
    [SerializeField] private float jumpSpeed = 20f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashTime = 0.5f;

    [Space]
    [SerializeField] private SlamSkill slam = null;

    private int LeanTweenID = -1;


    private void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;

        slamSpeed = slam.slamSpeed;
    }


    private void Update()
    {
        if (!state.isDashingState && !state.isSlammingState && !state.isKnockbackedState && !state.isHealingState)
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
        if (input.right)
        {
            if (horizontalSpeed < movementSpeed)
            {
                horizontalSpeed += movementSpeedUp * Time.deltaTime;
            }
            else
            {
                horizontalSpeed = movementSpeed;
            }
        }
        else if (input.left)
        {
            if (horizontalSpeed > -movementSpeed)
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
            horizontalSpeed = 0f;
        }
    }


    private void CheckJump()
    {
        if (!(LeanTweenID > 0 && LeanTween.isTweening(LeanTweenID)))
        {
            verticalSpeed = _rigidbody.velocity.y;
        }

        if (input.jumpDown)
        {
            if (state.isGroundedState)
            {
                state.SetJumpingState();

                verticalSpeed = jumpSpeed;
            }
            else if ((state.isJumpingState || state.isFallingState) && state.hasDoubleJump && !doubleJumped)
            {
                state.SetJumpingState();

                if (LeanTweenID > 0 && LeanTween.isTweening(LeanTweenID))
                {
                    LeanTween.cancel(LeanTweenID);
                    LeanTweenID = -1;
                }
                verticalSpeed = jumpSpeed;
                doubleJumped = true;
            }
        }
        else if (input.jumpUp && verticalSpeed > 0f)
        {
            LeanTweenID = LeanTween.value(verticalSpeed, 0f, 0.1f)
                .setOnUpdate((float v) => { verticalSpeed = v; })
                .setOnComplete(() => { LeanTweenID = -1; }).id;
        }
    }


    private void CheckSlam()
    {
        if(input.down && (state.isJumpingState || state.isFallingState) && state.hasSlam)
        {
            slam.OnSlamStart();
        }
    }


    private void CheckDash()
    {
        if (((input.dashRight && canDashRight) || (input.dashLeft && canDashLeft)) && state.hasDash)
        {
            isDashingThroughWall = true;
            isDashingRight = input.dashRight ? true : false;
            StartCoroutine(Dash(input.dashRight ? 1 : -1));
        }
        else if ((input.dashRight || input.dashLeft) && state.hasDash && !dashedInAir)
        {
            isDashingThroughWall = false;
            dashedInAir = state.isGroundedState ? false : true;
            isDashingRight = input.dashRight ? true : false;
            StartCoroutine(Dash(input.dashRight ? 1 : -1));
        }
    }


    IEnumerator Dash(int direction)
    {
        state.isDashingState = true;
        verticalSpeed = 0f;
        float gravityScaleCopy = _rigidbody.gravityScale;
        _rigidbody.gravityScale = 0f;
        state.EnableInvulnerability();
        float t = dashTime;

        // better shrink player
        transform.localScale = new Vector3(0.3f, 0.3f, 1f);

        while (state.isDashingState && t > 0f)
        {
            horizontalSpeed = direction * dashSpeed;

            if (!isDashingThroughWall) t -= Time.deltaTime;

            yield return null;
        }

        state.isDashingState = false;
        _rigidbody.gravityScale = gravityScaleCopy;
        transform.localScale = Vector3.one;
        state.DisableInvulnerability(dashTime);
    }


    private void ApplyMovement()
    {
        if (state.isSlammingState) verticalSpeed = -slamSpeed;
        if (state.isKnockbackedState) return;
        _rigidbody.velocity = new Vector2(horizontalSpeed, verticalSpeed);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Platform"))
        {
            return;
        }

        state.isDashingState = false;
        if (state.isDashingState && state.hasUpgradedDash && collision.collider.CompareTag("DestroyableWall"))
        {
            collision.collider.GetComponent<CustomDestroy>().Destroy();
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Trigger"))
        {
            if (collider.GetComponent<DashTrigger>().isRight)
            {
                if (state.isDashingState)
                {
                    if (!isDashingRight)
                        isDashingThroughWall = true;
                    else
                        state.isDashingState = false;
                }

                canDashLeft = true;
            }
            else
            {
                if (state.isDashingState)
                {
                    if (isDashingRight)
                        isDashingThroughWall = true;
                    else
                        state.isDashingState = false;
                }

                canDashRight = true;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Trigger"))
        {
            canDashLeft = canDashRight = false;
        }
    }


    public void OnGrounded(GameObject go = null)
    {
        doubleJumped = false;
        dashedInAir = false;

        if (state.isSlammingState)
        {
            slam.OnSlamEnd(go);
        }
    }
}