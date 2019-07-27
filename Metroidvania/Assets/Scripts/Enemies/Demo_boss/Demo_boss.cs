using System.Collections;
using UnityEngine;

public class Demo_boss : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private int secondPhaseHP = 10;
    [SerializeField] private int thirdPhaseHP = 5;
    [SerializeField] private float decisionTime = 1f;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private BossHealthManager healthManager = null;
    [SerializeField] private GameObject phaseColliders = null;
    [SerializeField] private GameObject chargeColliders = null;

    [Header("Moving")]
    [SerializeField] private float firstMovementSpeed = 5f;
    [SerializeField] private float firstMoveDistance = 20f;
    [SerializeField] private float secondMovementSpeed = 6f;
    [SerializeField] private float secondMoveDistance = 25f;
    [SerializeField] private float thirdMovementSpeed = 7f;

    [Header("Shooting")]
    [SerializeField] private Transform projectile = null;
    [SerializeField] private float forceVariation = 0.2f;
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
    [SerializeField] private float chargeSpeed = 20f;

    [Header("Rage")]
    [SerializeField] private float timeBetweenRageProjectiles = 0.05f;
    [SerializeField] private int firstRageProjectiles = 20;
    [SerializeField] private int secondRageProjectiles = 30;


    private float movementSpeed;
    private float moveDistance;
    private int minProjectiles;
    private int maxProjectiles;
    private int rageProjectiles;

    [SerializeField] private Transform player;
    private bool isDeciding = false;
    private bool isMoving = false;
    private bool isCharging = false;
    private bool isRaging = false;
    private bool isStunned = false;
    private float actionTime = 0f;
    private float destination = 0f;
    private int direction = 1;
    private int phase = 1;
    private int shootingSequence = 0;


    public void AssignPlayer(Transform p)
    {
        player = p;
    }


    public void HitInWeakPoint()
    {
        if (phase == 1 || phase == 2)
        {
            StartCoroutine(Rage());
        }
        else
        {
            healthManager.Death();
        }
    }


    private void Start()
    {
        SetPhase(1);
    }


    private void Update()
    {
        if (isRaging || isMoving || isCharging) return;

        if (isStunned)
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
        phaseColliders.SetActive(true);
        chargeColliders.SetActive(false);
        phase = phaseNum;

        if (phaseNum == 1)
        {
            movementSpeed = firstMovementSpeed;
            moveDistance = firstMoveDistance;
            minProjectiles = firstMinProjectiles;
            maxProjectiles = firstMaxProjectiles;
        }
        else if (phaseNum == 2)
        {
            movementSpeed = secondMovementSpeed;
            moveDistance = secondMoveDistance;
            minProjectiles = secondMinProjectiles;
            maxProjectiles = secondMaxProjectiles;
        }
        else
        {
            movementSpeed = thirdMovementSpeed;
            minProjectiles = thirdMinProjectiles;
            maxProjectiles = thirdMaxProjectiles;
        }
    }


    private void ChooseAction()
    {
        direction = player.position.x < transform.position.x ? -1 : 1;

        // PHASE 1
        if (phase == 1 && healthManager.currentHP > secondPhaseHP)
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
        else if (phase == 1 && healthManager.currentHP <= secondPhaseHP)
        {
            StartCoroutine(Charge());
        }

        // PHASE 2
        else if (phase == 2 && healthManager.currentHP > thirdPhaseHP)
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
        else if (phase == 2 && healthManager.currentHP <= thirdPhaseHP)
        {
            StartCoroutine(Charge());
        }

        // PHASE 3
        else if (phase == 3 && healthManager.currentHP > 1)
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
        else
        {
            StartCoroutine(Charge());
        }

        isDeciding = false;
    }


    private IEnumerator Move()
    {
        if (phase == 3)
        {
            destination = player.transform.position.x;
        }
        else
        {
            destination = transform.position.x + moveDistance * direction;
        }

        isMoving = true;

        while (isMoving)
        {
            if ((direction == 1 && transform.position.x < destination) || (direction == -1 && transform.position.x > destination))
            {
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
    }


    private void Shoot()
    {
        int num = Random.Range(minProjectiles, maxProjectiles);
        for (int i = 0; i < num; i++)
        {
            Rigidbody2D rb = Instantiate(projectile, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
            float angle = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle) * direction, Mathf.Sin(angle));
            float distance = Mathf.Abs(player.position.x - transform.position.x);
            float force = Mathf.Sqrt(-Physics2D.gravity.y * rb.gravityScale * distance / Mathf.Sin(Mathf.PI / 2f));
            force = Random.Range(force * (1f - forceVariation), force * (1f + forceVariation));
            rb.AddForce(dir * force, ForceMode2D.Impulse);
        }
    }


    private IEnumerator Charge()
    {
        isCharging = true;
        phaseColliders.SetActive(false);
        chargeColliders.SetActive(true);

        while (isCharging)
        {
            _rigidbody.velocity = new Vector2(chargeSpeed * direction, _rigidbody.velocity.y);

            yield return new WaitForEndOfFrame();
        }
    }


    private void StopCharging()
    {
        isCharging = false;
        isStunned = true;
    }

    private IEnumerator Rage()
    {
        isRaging = true;
        direction = player.position.x < transform.position.x ? -1 : 1;

        if (phase == 1) rageProjectiles = firstRageProjectiles;
        else rageProjectiles = secondRageProjectiles;

        while (shootingSequence < rageProjectiles)
        {
            Rigidbody2D rb = Instantiate(projectile, transform.position, transform.rotation).GetComponent<Rigidbody2D>();
            float angle = Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle) * direction, Mathf.Sin(angle));
            float distance = Mathf.Abs(player.position.x - transform.position.x);
            float force = Mathf.Sqrt(-Physics2D.gravity.y * rb.gravityScale * distance / Mathf.Sin(Mathf.PI / 2f));
            force = Random.Range(force * (1f - forceVariation), force * (1f + forceVariation));
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
