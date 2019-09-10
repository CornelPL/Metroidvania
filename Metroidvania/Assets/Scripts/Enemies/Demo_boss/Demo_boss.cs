using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class Demo_boss : MonoBehaviour
{
    [Header( "General" )]
    [SerializeField] private int secondPhaseHP = 10;
    [SerializeField] private int thirdPhaseHP = 5;
    [SerializeField] private int touchDamage = 1;
    [SerializeField] private float decisionTime = 1f;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private BossHealthManager healthManager = null;
    [SerializeField] private Animator _animator = null;

    [Header( "Moving" )]
    [SerializeField] private float firstMovementSpeed = 5f;
    [SerializeField] private float firstMoveDistance = 20f;
    [SerializeField] private float secondMovementSpeed = 6f;
    [SerializeField] private float secondMoveDistance = 25f;
    [SerializeField] private float thirdMovementSpeed = 7f;

    [Header( "Shooting" )]
    [SerializeField] private Transform shootPosition = null;
    [SerializeField] private Transform projectile = null;
    [SerializeField] private float forceVariation = 0.2f;
    [SerializeField] private float angleVariation = 0.2f;
    [SerializeField] private int firstMinProjectiles = 5;
    [SerializeField] private int firstMaxProjectiles = 10;
    [SerializeField] private int firstShootingSequence = 3;
    [SerializeField] private int secondMinProjectiles = 5;
    [SerializeField] private int secondMaxProjectiles = 10;
    [SerializeField] private int secondShootingSequence = 2;
    [SerializeField] private int thirdMinProjectiles = 5;
    [SerializeField] private int thirdMaxProjectiles = 10;
    [SerializeField] private int thirdShootingSequence = 1;

    [Header( "Armoring" )]
    [SerializeField] private float armoringTime = 2f;
    [SerializeField] private ParticleSystem[] armoringParticles = null;
    [SerializeField] private GameObject forceField = null;

    [Header( "Charge" )]
    [SerializeField] private float stunTime = 2f;
    [SerializeField] private float chargeSpeed = 20f;
    [SerializeField] private int chargeDamage = 2;
    [SerializeField] private float chargeKnockbackMultiplier = 2f;
    [SerializeField] private UnityEvent OnStun = null;
    [SerializeField] private Vector2Event EarthquakeEvent = null;

    [Header( "Rage" )]
    [SerializeField] private float timeBetweenRageProjectiles = 0.05f;
    [SerializeField] private int firstRageProjectiles = 20;
    [SerializeField] private int secondRageProjectiles = 30;
    [SerializeField] private ParticleSystem[] rageParticles = null;
    [SerializeField] private CinemachineImpulseSource OnRageImpulse = null;


    private float movementSpeed;
    private float moveDistance;
    private int minProjectiles;
    private int maxProjectiles;
    private int rageProjectiles;
    private int sequences;
    private int currentSequence;

    private Transform player;
    private bool isDeciding = false;
    private bool isMoving = false;
    private bool isCharging = false;
    private bool isRaging = false;
    private bool isStunned = false;
    private bool isShooting = false;
    private bool isArmoring = false;
    private bool isArmored = false;
    private bool wasShooting = false;
    private float actionTime = 0f;
    private float destination = 0f;
    private int direction = 1;
    private int phase = 1;


    public void AssignPlayer( Transform p )
    {
        player = p;
    }


    public void HitInWeakPoint()
    {
        if ( phase == 1 || phase == 2 )
        {
            SetStunned( false );
            isRaging = true;
            isArmored = false;
            _animator.SetBool( "isArmored", false );
        }
        else
        {
            healthManager.Death();
        }
    }


    public void ShootProjectiles()
    {
        direction = player.position.x < transform.position.x ? -1 : 1;

        int num = Random.Range( minProjectiles, maxProjectiles );
        for ( int i = 0; i < num; i++ )
        {
            ShootProjectile();
        }

        currentSequence++;

        if ( currentSequence == sequences )
        {
            currentSequence = 0;
            isShooting = false;
            _animator.SetBool( "isShooting", false );
            wasShooting = true;
        }
    }


    public void Charge()
    {
        isCharging = true;
        StartCoroutine( ChargeCoroutine() );
    }


    public void CheckPhaseHP()
    {
        if ( (phase == 1 && healthManager.currentHP <= secondPhaseHP) ||
             (phase == 2 && healthManager.currentHP <= thirdPhaseHP) ||
             (phase == 3 && healthManager.currentHP <= 0) )
        {
            ArmorUp();
        }
    }


    public void StartRage()
    {
        StartCoroutine( Rage() );
    }


    public void InvokeEarthquake()
    {
        EarthquakeEvent.Broadcast( gameObject, transform.position );
    }


    private void Start()
    {
        SetPhase( 1 );
    }


    private void Update()
    {
        if ( isRaging || isMoving || isCharging || isShooting || isArmoring ) return;

        if ( isStunned )
        {
            if ( actionTime < stunTime )
            {
                actionTime += Time.deltaTime;
            }
            else
            {
                actionTime = 0f;
                SetStunned( false );
            }
        }
        else if ( !isDeciding )
        {
            Invoke( "ChooseAction", decisionTime );
            isDeciding = true;
        }
    }


    private void SetStunned (bool b )
    {
        isStunned = b;
        _animator.SetBool( "isStunned", b );
    }


    private void SetPhase( int phaseNum )
    {
        phase = phaseNum;

        if ( phaseNum == 1 )
        {
            movementSpeed = firstMovementSpeed;
            moveDistance = firstMoveDistance;
            minProjectiles = firstMinProjectiles;
            maxProjectiles = firstMaxProjectiles;
            sequences = firstShootingSequence;
        }
        else if ( phaseNum == 2 )
        {
            movementSpeed = secondMovementSpeed;
            moveDistance = secondMoveDistance;
            minProjectiles = secondMinProjectiles;
            maxProjectiles = secondMaxProjectiles;
            sequences = secondShootingSequence;
        }
        else
        {
            movementSpeed = thirdMovementSpeed;
            minProjectiles = thirdMinProjectiles;
            maxProjectiles = thirdMaxProjectiles;
            sequences = thirdShootingSequence;
        }
    }


    private void ChooseAction()
    {
        direction = player.position.x < transform.position.x ? -1 : 1;

        if ( isArmored )
        {
            LoadCharge();
        }
        else if ( wasShooting )
        {
            StartCoroutine( Move() );
        }
        else
        {
            Shoot();
        }

        isDeciding = false;
    }


    private void ArmorUp()
    {
        if ( isMoving )
        {
            StopMoving();
        }

        _animator.SetBool( "isShooting", false );
        isShooting = false;

        isDeciding = false;

        isRaging = false;

        forceField.SetActive( false );
        Invoke( "SetArmored", armoringTime );
        foreach ( ParticleSystem p in armoringParticles )
        {
            p.Play();
        }
        isArmoring = true;

        _animator.Play( "Demo_boss_idle" );
    }


    private void SetArmored()
    {
        isArmored = true;
        _animator.SetBool( "isArmored", true );

        isArmoring = false;
        foreach ( ParticleSystem p in armoringParticles )
        {
            p.Stop();
        }
        forceField.SetActive( true );
    }


    private IEnumerator Move()
    {
        if ( phase == 3 )
        {
            destination = player.transform.position.x;
        }
        else
        {
            destination = transform.position.x + moveDistance * direction;
        }

        isMoving = true;
        _animator.SetBool( "isMoving", true );

        while ( isMoving )
        {
            if ( (direction == 1 && transform.position.x < destination) || (direction == -1 && transform.position.x > destination) )
            {
                _rigidbody.velocity = new Vector2( movementSpeed * direction, _rigidbody.velocity.y );
            }
            else
            {
                StopMoving();
            }

            yield return new WaitForEndOfFrame();
        }

        wasShooting = false;
    }


    private void StopMoving()
    {
        _rigidbody.velocity = new Vector2( 0f, 0f );
        isMoving = false;
        _animator.SetBool( "isMoving", false );
    }


    private void ShootProjectile()
    {
        Rigidbody2D rb = Instantiate( projectile, shootPosition.position, transform.rotation ).GetComponent<Rigidbody2D>();

        Vector2 vectorToPlayer = player.position - shootPosition.position;

        float angleToPlayer = Mathf.Atan2( vectorToPlayer.y, vectorToPlayer.x ) * Mathf.Rad2Deg;
        if ( angleToPlayer < -90f )
            angleToPlayer = -(180f + angleToPlayer);
        else if ( angleToPlayer > 90f )
        {
            angleToPlayer = 180f - angleToPlayer;
        }
        float angle = 45;
        if ( angleToPlayer < 0f )
        {
            angle -= angleToPlayer / 2f;
            vectorToPlayer.y = 0f;
        }
        else
        {
            angle += angleToPlayer / 2f;
        }
        angle = Random.Range( angle * (1f - angleVariation), angle * (1f + angleVariation) );
        if ( angle > 89f ) angle = 89f;
        angle *= Mathf.Deg2Rad;

        Vector2 dir = new Vector2( Mathf.Cos( angle ) * direction, Mathf.Sin( angle ) );
        float distance = vectorToPlayer.magnitude;
        float force = Mathf.Sqrt( -Physics2D.gravity.y * rb.gravityScale * distance / Mathf.Sin( 2f * angle ) );
        force = Random.Range( force * (1f - forceVariation), force * (1f + forceVariation) );
        rb.AddForce( dir * force, ForceMode2D.Impulse );
    }


    private void Shoot()
    {
        isShooting = true;
        _animator.SetBool( "isShooting", true );
    }


    private void LoadCharge()
    {
        isCharging = true;
        _animator.SetTrigger( "charge" );
        wasShooting = false;
    }


    private IEnumerator ChargeCoroutine()
    {
        while ( isCharging )
        {
            _rigidbody.velocity = new Vector2( chargeSpeed * direction, _rigidbody.velocity.y );

            yield return new WaitForEndOfFrame();
        }
    }


    private void StopCharging()
    {
        isCharging = false;
        SetStunned( true );
        OnStun.Invoke();
    }


    private IEnumerator Rage()
    {
        isRaging = true;


        direction = player.position.x < transform.position.x ? -1 : 1;

        if ( phase == 1 ) rageProjectiles = firstRageProjectiles;
        else rageProjectiles = secondRageProjectiles;

        foreach (ParticleSystem p in rageParticles )
        {
            p.Play();
        }

        while ( currentSequence < rageProjectiles )
        {
            OnRageImpulse.GenerateImpulse();

            ShootProjectile();

            currentSequence++;
            yield return new WaitForSeconds( timeBetweenRageProjectiles );
        }

        foreach ( ParticleSystem p in rageParticles )
        {
            p.Stop();
        }

        isRaging = false;
        _animator.SetTrigger( "rageEnd" );
        currentSequence = 0;
        SetPhase( phase == 1 ? 2 : 3 );
    }


    private void OnCollisionEnter2D( Collision2D collision )
    {
        if ( collision.collider.CompareTag( "Wall" ) )
        {
            if ( isMoving )
            {
                StopMoving();
            }
            else if ( isCharging )
            {
                StopCharging();
            }
        }
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        if ( collider.CompareTag( "Player" ) )
        {
            int damage = touchDamage;
            float knockbackMultiplier = 1f;

            if ( isCharging )
            {
                damage = chargeDamage;
                knockbackMultiplier = chargeKnockbackMultiplier;
            }

            collider.GetComponent<PlayerHealthManager>().TakeDamage( damage, transform.position.x, knockbackMultiplier );
        }
    }
}
