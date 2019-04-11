using System.Collections;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private int initialHP = 100;
    [SerializeField] private float knockbackForce = 100f;
    [SerializeField] private float knockbackTime = 0.5f;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private Rigidbody2D _rigidbody = null;

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


    private IEnumerator Knockback(float xPos)
    {
        PlayerState.instance.isKnockbackedState = true;
        PlayerState.instance.EnableInvulnerability();
        _rigidbody.AddForce(new Vector2(0.5f * Mathf.Sign(transform.position.x - xPos), 1f) * knockbackForce);

        yield return new WaitForSeconds(knockbackTime);
        PlayerState.instance.isKnockbackedState = false;

        PlayerState.instance.DisableInvulnerability(invulnerabilityTime);
    }


    public void TakeDamage(int damage, float xPos)
    {
        HP -= damage;

        StartCoroutine(Knockback(xPos));

        if (HP < 0)
        {
            Death();
        }
    }
}
