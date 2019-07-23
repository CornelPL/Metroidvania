using System.Collections;
using UnityEngine;

public class Demo_boss : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private int maxHP = 20;
    [SerializeField] private int secondPhaseHP = 10;
    [SerializeField] private int thirdPhaseHP = 5;
    [SerializeField] private float decisionTime = 1f;
    [SerializeField] private Rigidbody2D _rigidbody = null;

    [Header("Moving")]
    [SerializeField] private float firstMovementSpeed = 5f;
    [SerializeField] private float firstMoveTime = 2f;
    [SerializeField] private float secondMovementSpeed = 5f;
    [SerializeField] private float secondMoveTime = 2f;
    [SerializeField] private float thirdMovementSpeed = 5f;
    [SerializeField] private float thirdMoveTime = 2f;

    [Header("Shooting")]
    [SerializeField] private Transform projectile = null;
    [SerializeField] private float minShootForce = 5f;
    [SerializeField] private float maxShootForce = 7f;
    [SerializeField] private float minAngle = 30f;
    [SerializeField] private float maxAngle = 80f;
    [SerializeField] private int firstMinProjectiles = 5;
    [SerializeField] private int firstMaxProjectiles = 10;
    [SerializeField] private int secondMinProjectiles = 5;
    [SerializeField] private int secondMaxProjectiles = 10;
    [SerializeField] private int thirdMinProjectiles = 5;
    [SerializeField] private int thirdMaxProjectiles = 10;

    [Header("Charge")]
    [SerializeField] private float stunTime = 2f;
    [SerializeField] private float firstChargeSpeed = 10f;
    [SerializeField] private float secondChargeSpeed = 10f;
    [SerializeField] private float thirdChargeSpeed = 10f;

    [Header("Rage")]
    [SerializeField] private float timeBetweenRageProjectiles = 0.05f;
    [SerializeField] private int firstRageProjectiles = 20;
    [SerializeField] private int secondRageProjectiles = 30;


    private float movementSpeed;
    private float moveTime;
    private int minProjectiles;
    private int maxProjectiles;
    private float chargeSpeed;
    private int rageProjectiles;

    private int currentHP;
    [SerializeField] private Transform player;
    private bool isDeciding = false;
    private bool isMoving = false;
    private bool isCharging = false;
    private bool isRaging = false;
    private bool isStunned = false;
    private float actionTime = 0f;
    private int direction = 1;
    private int phase = 1;
    private int shootingSequence = 0;


    public void AssignPlayer(Transform p)
    {
        player = p;
    }


    private void Start()
    {
        currentHP = maxHP;
        SetPhase(1);
    }


    private void Update()
    {
        if (isRaging || isMoving) return;

        if (isCharging)
        {
            Charge();
        }
        else if (isStunned)
        {
            if (actionTime < stunTime)
            {
                actionTime += Time.deltaTime;
            }
            else
            {
                actionTime = 0f;
                isStunned = false;
            }
        }
        else if (!isDeciding)
        {
            Invoke("ChooseAction", decisionTime);
            isDeciding = true;
        }
    }


    private void SetPhase(int phaseNum)
    {
        if (phaseNum == 1)
        {
            phase = 1;
            movementSpeed = firstMovementSpeed;
            moveTime = firstMoveTime;
            minProjectiles = firstMinProjectiles;
            maxProjectiles = firstMaxProjectiles;
            chargeSpeed = firstChargeSpeed;
        }
        else if (phaseNum == 2)
        {
            phase = 2;
            movementSpeed = secondMovementSpeed;
            moveTime = secondMoveTime;
            minProjectiles = secondMinProjectiles;
            maxProjectiles = secondMaxProjectiles;
            chargeSpeed = secondChargeSpeed;
        }
        else
        {
            phase = 3;
            movementSpeed = thirdMovementSpeed;
            moveTime = thirdMoveTime;
            minProjectiles = thirdMinProjectiles;
            maxProjectiles = thirdMaxProjectiles;
            chargeSpeed = thirdChargeSpeed;
        }
    }


    private void ChooseAction()
    {
        direction = player.position.x < transform.position.x ? -1 : 1;
        if (phase == 1 && currentHP > secondPhaseHP)
        {
            if (shootingSequence < 3)
            {
                Shoot();
                shootingSequence++;
            }
            else
            {
                StartCoroutine(Move());
                shootingSequence = 0;
            }
        }
        else if (phase == 1 && currentHP <= secondPhaseHP)
        {
            isCharging = true;
        }
        else if (phase == 2 && currentHP > thirdPhaseHP)
        {
            if (shootingSequence < 2)
            {
                Shoot();
                shootingSequence++;
            }
            else
            {
                StartCoroutine(Move());
                shootingSequence = 0;
            }
        }
        else if (phase == 2 && currentHP <= thirdPhaseHP)
        {
            isCharging = true;
        }
        else
        {

        }

        isDeciding = false;
    }

    //change to move distance not time
    private IEnumerator Move()
    {
        isMoving = true;

        while (isMoving)
        {
            if (actionTime < moveTime)
            {
                actionTime += Time.deltaTime;
                _rigidbody.velocity = new Vector2(movementSpeed * direction, _rigidbody.velocity.y);
            }
            else
            {
                StopMoving();
            }

            yield return new WaitForEndOfFrame();
        }
    }


    private void StopMoving()
    {
        _rigidbody.velocity = new Vector2(0f, 0f);
        isMoving = false;
        actionTime = 0f;
    }


    private void Shoot()
    {
        int num = Random.Range(minProjectiles, maxProjectiles);
        for (int i = 0; i < num; i++)
        {
            float angle = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle) * direction, Mathf.Sin(angle));
            Rigidbody2D rb = Instantiate(projectile, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
            float force = Random.Range(minShootForce, maxShootForce);
            rb.AddForce(dir * force, ForceMode2D.Impulse);
        }
    }


    private void Charge()
    {
        _rigidbody.velocity = new Vector2(chargeSpeed * direction, _rigidbody.velocity.y);
    }


    private void StopCharging()
    {
        isCharging = false;
        isStunned = true;
    }


    private void StartRage()
    {
        isRaging = true;
        direction = player.position.x < transform.position.x ? -1 : 1;

        if (phase == 1) rageProjectiles = firstRageProjectiles;
        else rageProjectiles = secondRageProjectiles;

        StartCoroutine(Rage());
    }

    private IEnumerator Rage()
    {
        while (shootingSequence < rageProjectiles)
        {
            float angle = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle) * direction, Mathf.Sin(angle));
            Rigidbody2D rb = Instantiate(projectile, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
            float force = Random.Range(minShootForce, maxShootForce);
            rb.AddForce(dir * force, ForceMode2D.Impulse);

            shootingSequence++;
            yield return new WaitForSeconds(timeBetweenRageProjectiles);
        }

        isRaging = false;
        shootingSequence = 0;
        SetPhase(phase == 1 ? 2 : 3);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            if (isMoving)
            {
                StopMoving();
            }
            else if (isCharging)
            {
                StopCharging();
            }
        }
    }
}
