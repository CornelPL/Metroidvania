using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MyBox;


public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private int maxHP = 5;
    [SerializeField] private float knockbackForce = 100f;
    [SerializeField] private float knockbackTime = 0.5f;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private float healChargeTime = 1f;
    [SerializeField] private int pointsPercentLoseOnDeath = 30;
    [SerializeField, MustBeAssigned] private Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] private PolygonCollider2D _collider = null;
    [SerializeField, MustBeAssigned] private GameObject lostPoints = null;
    [SerializeField, MustBeAssigned] private Image[] barsBackgrounds = null;
    [SerializeField, MustBeAssigned] private Image[] bars = null;
    [SerializeField, MustBeAssigned] private Image fadeImage = null;
    [SerializeField] private float fadeTime = 1f;

    private InputController input;
    private PlayerState state;
    private PointsController pointsController;
    private int currentHP;
    private float healingTime = 0f;
    private float t = 0f;


    private void Awake()
    {
        currentHP = maxHP;
        UpdateBars();
        UpdateBarsBackgrounds();
    }


    private void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;
        pointsController = PointsController.instance;
    }


    private void Update()
    {
        if ( input.healDown && EnergyController.instance.isContainerFull && currentHP < maxHP && state.isGroundedState && !state.isRunningState && !state.isKnockbackedState )
        {
            state.isHealingState = true;
        }
        if ( input.healUp )
        {
            state.isHealingState = false;
            healingTime = 0f;
        }

        if ( state.isHealingState )
        {
            if ( healingTime < healChargeTime )
            {
                healingTime += Time.deltaTime;
            }
            else
            {
                state.isHealingState = false;
                healingTime = 0f;
                EnergyController.instance.EmptyContainer();
                currentHP++;
                UpdateBars();
            }
        }
    }


    private IEnumerator Death()
    {
        SetDead( true );

        int pointsLost = pointsController.points * pointsPercentLoseOnDeath / 100;
        pointsController.AddPoints( -pointsLost );

        // TODO: death particles

        Vector2 spawnPos = new Vector2( transform.position.x, transform.position.y + 1f );
        Instantiate( lostPoints, spawnPos, Quaternion.identity, null ).GetComponent<LostPoints>().points = pointsLost;

        t = 0f;
        while ( t < fadeTime )
        {
            t += Time.deltaTime;
            Color c = fadeImage.color;
            fadeImage.color = new Color( c.r, c.g, c.b, Mathf.Clamp01( t ) );
            yield return null;
        }

        transform.position = state.savePoint.position;
        state.room.OnPlayerDeath();

        while ( t > 0f )
        {
            t -= Time.deltaTime;
            Color c = fadeImage.color;
            fadeImage.color = new Color( c.r, c.g, c.b, Mathf.Clamp01( t ) );
            yield return null;
        }

        SetDead( false );
    }


    private void SetDead( bool b )
    {
        state.isDeadState = b;
        _rigidbody.simulated = !b;
        _collider.enabled = !b;
        if ( b )
        {
            _rigidbody.velocity = Vector2.zero;
        }
        else
        {
            currentHP = maxHP;
        }
    }


    private void OnCollisionEnter2D( Collision2D collision )
    {
        if ( collision.gameObject.CompareTag( "Spikes" ) )
        {
            StartCoroutine( Death() );
        }
    }


    private void UpdateBars()
    {
        for ( int i = 0; i < bars.Length; i++ )
        {
            bars[ i ].enabled = i < currentHP ? true : false;
        }
    }


    private void UpdateBarsBackgrounds()
    {
        for ( int i = 0; i < barsBackgrounds.Length; i++ )
        {
            barsBackgrounds[ i ].enabled = i < maxHP ? true : false;
        }
    }


    private IEnumerator Knockback( float xPos, float knockbackMultiplier )
    {
        PlayerState.instance.isKnockbackedState = true;
        PlayerState.instance.SetInvulnerable( true );
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce( new Vector2( 0.5f * Mathf.Sign( transform.position.x - xPos ), 1f ) * knockbackForce * knockbackMultiplier, ForceMode2D.Impulse );

        yield return new WaitForSeconds( knockbackTime );
        PlayerState.instance.isKnockbackedState = false;

        PlayerState.instance.SetInvulnerable( false, invulnerabilityTime );
    }


    public void TakeDamage( int damage, float xPos, float knockbackMultiplier = 1f )
    {
        currentHP -= damage;
        UpdateBars();

        StartCoroutine( Knockback( xPos, knockbackMultiplier ) );

        if ( currentHP <= 0 )
        {
            StartCoroutine( Death() );
        }
    }
}
