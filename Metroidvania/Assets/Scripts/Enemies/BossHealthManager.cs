using System.Collections;
using UnityEngine;

public class BossHealthManager : HealthManager
{
    [SerializeField] private float normalBrightness = 0.2f;
    [SerializeField] private float hitBrightness = 1f;
    [SerializeField] private float brightnessChangeTime = 0.5f;
    [SerializeField] private float deathTime = 3f;
    [SerializeField] private ParticleSystem[] deathParticles = null;
    [SerializeField] private ParticleSystem[] deathParticles2 = null;
    [SerializeField] private ParticleSystem[] deathParticles3 = null;


    override public void ChangeColorOnDamage()
    {
        LeanTween.value( gameObject, hitBrightness, normalBrightness, brightnessChangeTime ).setOnUpdate( ( float v ) => { spriteRenderer.material.SetFloat( "_Brightness", v ); } );
    }


    public void Death()
    {
        OnDeath.Invoke();
        StartCoroutine( DeathCoroutine() );
        StartCoroutine( DropPoints() );
    }


    private IEnumerator DeathCoroutine()
    {
        foreach ( ParticleSystem p in deathParticles )
        {
            p.Play();
        }

        yield return new WaitForSeconds( deathTime / 3f );

        foreach ( ParticleSystem p in deathParticles2 )
        {
            p.Play();
        }

        yield return new WaitForSeconds( deathTime / 3f );

        foreach ( ParticleSystem p in deathParticles3 )
        {
            p.Play();
        }

        yield return new WaitForSeconds( deathTime / 3f );

        foreach ( ParticleSystem p in deathParticles )
        {
            p.Stop();
        }

        foreach ( ParticleSystem p in deathParticles2 )
        {
            p.Stop();
        }

        foreach ( ParticleSystem p in deathParticles3 )
        {
            p.Stop();
        }

        // final explosion
    }


    private IEnumerator DropPoints()
    {
        int count = Random.Range( minPoints, maxPoints );

        for ( int i = 0; i < count; i++ )
        {
            Transform inst = Instantiate( point, transform.position, transform.rotation );
            Vector2 dropForce = Random.insideUnitCircle * pointsDropForce;
            dropForce.y = Mathf.Abs( dropForce.y );
            inst.GetComponent<Rigidbody2D>().AddForce( dropForce, ForceMode2D.Impulse );
            yield return new WaitForSeconds( deathTime / count );
        }
    }


    override public void TakeDamage( int damage )
    {
        currentHP -= damage;
        ChangeColorOnDamage();
        OnTakeDamage.Invoke();
    }
}