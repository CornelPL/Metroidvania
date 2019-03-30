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
        HP -= damage;

        if (HP < 0)
        {
            Death();
        }
    }
}
