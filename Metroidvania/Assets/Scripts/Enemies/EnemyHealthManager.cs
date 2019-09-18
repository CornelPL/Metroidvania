using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    [SerializeField] private Rigidbody2D _rigidbody = null;

    [HideInInspector] public bool isBeingKnockbacked = false;


    private void Death()
    {
        OnDeath.Invoke();
        DropPoints();
        Destroy(gameObject);
    }


    private void DropPoints()
    {
        int count = Random.Range(minPoints, maxPoints);
        for (int i = 0; i < count; i++)
        {
            Transform inst = Instantiate(point, transform.position, transform.rotation);
            Vector2 dropForce = Random.insideUnitCircle * pointsDropForce;
            dropForce.y = Mathf.Abs(dropForce.y);
            inst.GetComponent<Rigidbody2D>().AddForce(dropForce, ForceMode2D.Impulse);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Spikes"))
        {
            Death();
        }
    }


    public override void Knockback(Vector2 direction, float force)
    {
        isBeingKnockbacked = true;

        _rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }


    override public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP < 0)
        {
            Death();
        }
        else
        {
            ChangeColorOnDamage();
            OnTakeDamage.Invoke();
        }
    }
}