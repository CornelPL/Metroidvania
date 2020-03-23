using UnityEngine;
using MyBox;

public class EnemyHealthManager : HealthManager
{
    [SerializeField, MustBeAssigned] private Rigidbody2D _rigidbody = null;
    [SerializeField, MustBeAssigned] private GameObject smallHitEffect = null;
    [SerializeField, MustBeAssigned] private int bigHitTreshold = 5;
    [SerializeField, MustBeAssigned] private GameObject bigHitEffect = null;
    [SerializeField, MustBeAssigned] private GameObject deathEffect = null;

    [HideInInspector] public bool isBeingKnockbacked = false;
    [HideInInspector] public Vector2 hitDirection = Vector2.zero;


    private void OnDisable()
    {
        currentHP = initialHP;
    }


    private void Death( Vector2 shootDirection )
    {
        DropPoints();
        SpawnEffect( shootDirection, deathEffect );
        EnergyController.instance.AddEnergy( EnergyGain.OnKill );

        hitDirection = shootDirection;

        OnDeath.Invoke();

        this.enabled = false;
    }


    private void DropPoints()
    {
        int count = Random.Range(minPoints, maxPoints);
        Vector2 pos = new Vector2( transform.position.x, transform.position.y + 0.5f );
        for (int i = 0; i < count; i++)
        {
            GameObject inst = Instantiate(point, pos, transform.rotation);
            Vector2 dropForce = Random.insideUnitCircle * pointsDropForce;
            dropForce.y = Mathf.Abs(dropForce.y);
            inst.GetComponent<Rigidbody2D>().AddForce(dropForce, ForceMode2D.Impulse);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Spikes"))
        {
            Death( Vector2.up );
        }
    }


    public override void Knockback(Vector2 direction, float force )
    {
        isBeingKnockbacked = true;

        direction.y += 1f;
        direction.Normalize();

        _rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }


    override public void TakeDamage( Vector2 direction, int damage )
    {
        currentHP -= damage;

        if ( damage < bigHitTreshold )
        {
            SpawnEffect( direction, smallHitEffect );
        }
        else
        {
            SpawnEffect( direction, bigHitEffect );
        }

        ChangeColorOnDamage();
        EnergyController.instance.AddEnergy( damage );

        if ( currentHP <= 0)
        {
            Death( direction );
        }
        else
        {
            OnTakeDamage.Invoke();
        }
    }


    private void SpawnEffect( Vector2 direction, GameObject effect )
    {
        float angle = Mathf.Atan2( direction.y, direction.x ) * Mathf.Rad2Deg;

        Vector3 position = transform.position;
        Vector3 spawnPos = new Vector3( position.x, position.y + spriteRenderer.bounds.size.y / 2f, position.z );

        Instantiate( effect, spawnPos, Quaternion.Euler( 0f, 0f, angle ) );
    }
}