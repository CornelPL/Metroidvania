using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private int initialHP = 100;
    private int HP;


    private void Awake()
    {
        HP = initialHP;
    }


    private void Death()
    {
        // Implement Death
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            Death();
        }
    }


    public void TakeDamage(int damage)
    {
        if (PlayerState.instance.isDashingState) return;

        HP -= damage;

        // Knockback

        if (HP < 0)
        {
            Death();
        }
    }
}
