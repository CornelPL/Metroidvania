using System.Collections;
using UnityEngine;
using Cinemachine;


public class BossHealthManager : HealthManager
{
    [SerializeField] private float normalBrightness = 0.2f;
    [SerializeField] private float hitBrightness = 1f;
    [SerializeField] private float brightnessChangeTime = 0.5f;
    [SerializeField] private float deathTime = 3f;
    [SerializeField] private Transform pointsDropPosition = null;
    [SerializeField] private SpriteRenderer sprite = null;
    [SerializeField] private GameObject _collider = null;
    [SerializeField] private ParticleSystem[] deathParticles = null;
    [SerializeField] private GameObject eyesLight = null;
    [SerializeField] private GameObject firstExplosionOneShot = null;
    [SerializeField] private ParticleSystem[] firstExplosionLooping = null;
    [SerializeField] private GameObject secondExplosionOneShot = null;
    [SerializeField] private ParticleSystem[] secondExplosionLooping = null;
    [SerializeField] private GameObject thirdExplosionOneShot = null;
    [SerializeField] private ParticleSystem[] thirdExplosionLooping = null;
    [SerializeField] private GameObject lastExplosionForceField = null;
    [SerializeField] private CinemachineImpulseSource LastExplosionImpulse = null;
    [SerializeField] private Vector2Event shockwaveEventSource = null;
    [SerializeField] private FloatEvent shockwaveEventForce = null;
    [SerializeField] private float shockwaveForce = 0.1f;
    [SerializeField] private float shockwaveTime = 0.7f;

    [Header("Armor Parts")]
    [SerializeField] private GameObject[] armorParts = null;
    [SerializeField] private int numOfParts = 12;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 1.7f;
    [SerializeField] private float shootForce = 100f;


    override protected void ChangeColorOnDamage()
    {
        LeanTween.value( gameObject, hitBrightness, normalBrightness, brightnessChangeTime ).setOnUpdate( ( float v ) => { spriteRenderer.material.SetFloat( "_Brightness", v ); } );
    }


    public void Death()
    {
        OnDeath.Invoke();
        StartCoroutine( DeathCoroutine() );
        StartCoroutine( DropPoints() );
        EnergyController.instance.AddEnergy( EnergyGain.OnKillBoss );
    }


    private IEnumerator DeathCoroutine()
    {
        eyesLight.SetActive( true );

        foreach ( ParticleSystem p in deathParticles )
        {
            p.Play();
        }

        LastExplosionImpulse.GenerateImpulse();

        yield return new WaitForSeconds( deathTime / 4f );

        firstExplosionOneShot.SetActive( true );

        foreach ( ParticleSystem p in firstExplosionLooping )
        {
            p.Play();
        }

        LastExplosionImpulse.GenerateImpulse();

        yield return new WaitForSeconds( deathTime / 4f );

        secondExplosionOneShot.SetActive( true );

        foreach ( ParticleSystem p in secondExplosionLooping )
        {
            p.Play();
        }

        LastExplosionImpulse.GenerateImpulse();

        yield return new WaitForSeconds( deathTime / 4f );

        thirdExplosionOneShot.SetActive( true );

        foreach ( ParticleSystem p in thirdExplosionLooping )
        {
            p.Play();
        }

        LastExplosionImpulse.GenerateImpulse();

        yield return new WaitForSeconds( deathTime / 4f );

        foreach ( ParticleSystem p in deathParticles )
        {
            p.Stop();
        }

        foreach ( ParticleSystem p in firstExplosionLooping )
        {
            p.Stop();
        }

        foreach ( ParticleSystem p in secondExplosionLooping )
        {
            p.Stop();
        }

        foreach ( ParticleSystem p in thirdExplosionLooping )
        {
            p.Stop();
        }

        LastExplosionImpulse.GenerateImpulse();

        eyesLight.SetActive( false );
        firstExplosionOneShot.SetActive( false );
        secondExplosionOneShot.SetActive( false );
        thirdExplosionOneShot.SetActive( false );
        lastExplosionForceField.SetActive( true );

        sprite.enabled = false;
        _collider.SetActive( false );

        for ( int i = 0; i < numOfParts; i++ )
        {
            GameObject objectToInstantiate = armorParts[ Random.Range( 0, armorParts.Length ) ];
            GameObject instantiated = Instantiate( objectToInstantiate, pointsDropPosition.position, Quaternion.AngleAxis( Random.Range( 0f, 360f ), Vector3.forward ), null );
            float randomScale = Random.Range( minScale, maxScale );
            instantiated.transform.localScale = new Vector3( randomScale, randomScale, 1f );
            Vector2 direction = Random.insideUnitCircle;
            direction.y += 1.5f;
            instantiated.GetComponent<Rigidbody2D>().mass = randomScale;
            instantiated.GetComponent<Rigidbody2D>().AddTorque( Random.Range( 0f, 5f ), ForceMode2D.Impulse );
            instantiated.GetComponent<Rigidbody2D>().AddForce( direction * shootForce, ForceMode2D.Impulse );
        }

        StartCoroutine( GenerateShockwave() );
    }


    private IEnumerator GenerateShockwave()
    {
        float t = 0f;

        while ( t < shockwaveTime )
        {
            t += Time.deltaTime;

            shockwaveEventSource.Broadcast( gameObject, pointsDropPosition.position );
            shockwaveEventForce.Broadcast( gameObject, shockwaveForce );

            yield return new WaitForSeconds( 0.05f );
        }
    }


    private IEnumerator DropPoints()
    {
        int count = Random.Range( minPoints, maxPoints );

        for ( int i = 0; i < count; i++ )
        {
            GameObject inst = Instantiate( point, pointsDropPosition.position, transform.rotation );
            Vector2 dropForce = Random.insideUnitCircle * pointsDropForce;
            dropForce.y = Mathf.Abs( dropForce.y );
            inst.GetComponent<Rigidbody2D>().AddForce( dropForce, ForceMode2D.Impulse );
            yield return new WaitForSeconds( deathTime / count );
        }
    }


    override public void TakeDamage( Vector2 direction, int damage )
    {
        currentHP -= damage;
        ChangeColorOnDamage();
        OnTakeDamage.Invoke();
    }
}