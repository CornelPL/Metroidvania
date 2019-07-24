using UnityEngine;

public class EnemyHealthManager : HealthManager
{
    private const float knockbackForce = 5000f;

    [SerializeField] private Rigidbody2D _rigidbody = null;

    [HideInInspector] public bool isBeingKnockbacked = false;


    private void Death()
    {
        Debug.Log("Enemy dead");
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


    public void Knockback(float xPos, float force = knockbackForce)
    {
        isBeingKnockbacked = true;

        _rigidbody.AddForce(new Vector2(0.5f * Mathf.Sign(transform.position.x - xPos), 1f) * force);
    }


    override public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP < 0)
        {
            Death();
        }
    }
}