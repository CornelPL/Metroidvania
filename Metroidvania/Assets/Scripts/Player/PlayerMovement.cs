using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidbody = null;

    private InputController input;
    private PlayerState state;
    private float horizontalSpeed = 0f;
    private float verticalSpeed = 0f;
    private bool doubleJumped = false;
    private bool canDashRight = false;
    private bool canDashLeft = false;
    private bool isDashingThroughWall = false;
    private bool isDashingRight = false;
    private bool dashedInAir = false;

    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpSpeed = 20f;
    [SerializeField] private float flyingSpeed = 2f;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashTime = 0.5f;

    [Header("Slam")]
    [SerializeField] private float slamSpeed = 2f;
    [SerializeField] private float slamRange = 5f;
    [SerializeField] private float itemsKnockbackForce = 10f;
    [SerializeField] private float enemiesKnockbackForce = 10f;
    public LayerMask slamMask;

    private int LeanTweenID = -1;


    private void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;
    }


    private void Update()
    {
        if (!state.isDashingState && !state.isSlammingState && !state.isKnockbackedState)
        {
            CheckMovement();

            CheckFlying();

            CheckJump();

            CheckSlam();

            CheckDash();
        }

        ApplyMovement();
    }


    private void CheckMovement()
    {
        if (input.right) horizontalSpeed = movementSpeed;
        else if (input.left) horizontalSpeed = -movementSpeed;
        else horizontalSpeed = 0f;
    }


    private void CheckJump()
    {
        if (!(LeanTweenID > 0 && LeanTween.isTweening(LeanTweenID)) && !state.isFlyingState)
        {
            verticalSpeed = _rigidbody.velocity.y;
        }

        if (input.jumpDown)
        {
            if (state.isGroundedState)
            {
                verticalSpeed = jumpSpeed;
                state.isJumpingState = true;
            }
            else if ((state.isJumpingState || state.isFallingState) && state.hasDoubleJump && !doubleJumped)
            {
                if (LeanTweenID > 0 && LeanTween.isTweening(LeanTweenID))
                {
                    LeanTween.cancel(LeanTweenID);
                    LeanTweenID = -1;
                }
                state.isJumpingState = true;
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


    private void CheckFlying()
    {
        if (state.hasFlying && input.flying && _rigidbody.velocity.y < 0f)
        {
            state.isFlyingState = true;
            verticalSpeed = -flyingSpeed;
        }
        
        if (input.flyingUp)
        {
            state.isFlyingState = false;
        }
    }


    private void CheckSlam()
    {
        if(input.down && (state.isJumpingState || state.isFallingState) && state.hasSlam)
        {
            state.isSlammingState = true;
            state.EnableInvulnerability();
        }
    }


    private void Slam()
    {
        state.DisableInvulnerability(0.5f);

        Collider2D[] objectsInRange = Physics2D.OverlapCircleAll(transform.position, slamRange, slamMask);

        for (int i = 0; i < objectsInRange.Length; i++)
        {
            Vector2 direction = objectsInRange[i].transform.position - new Vector3(transform.position.x, transform.position.y - 1f);
            direction.Normalize();
            if (objectsInRange[i].CompareTag("Item"))
            {
                objectsInRange[i].attachedRigidbody.AddForce(direction * itemsKnockbackForce, ForceMode2D.Impulse);
            }
            else if (objectsInRange[i].CompareTag("Enemy"))
            {
                objectsInRange[i].GetComponent<EnemyHealthManager>().Knockback(transform.position.x, enemiesKnockbackForce);
            }
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
        if (state.isDashingState && (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Ground")))
        {
            state.isDashingState = false;
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


    public void OnGrounded()
    {
        state.isGroundedState = true;
        state.isJumpingState = false;
        doubleJumped = false;
        dashedInAir = false;

        if (state.isSlammingState)
        {
            state.isSlammingState = false;
            verticalSpeed = 0f;
            Slam();
        }
    }
}