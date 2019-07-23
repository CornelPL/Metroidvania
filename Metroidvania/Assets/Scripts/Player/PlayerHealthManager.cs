using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private int maxHP = 5;
    [SerializeField] private float knockbackForce = 100f;
    [SerializeField] private float knockbackTime = 0.5f;
    [SerializeField] private float invulnerabilityTime = 1f;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private Image[] barsBackgrounds = null;
    [SerializeField] private Image[] bars = null;

    private int currentHP;


    private void Awake()
    {
        currentHP = maxHP;
        UpdateBars();
        UpdateBarsBackgrounds();
    }


    private void Death()
    {
        Debug.Log("DEAD");
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spikes"))
        {
            Death();
        }
    }


    private void UpdateBars()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].enabled = i < currentHP ? true : false;
        }
    }


    private void UpdateBarsBackgrounds()
    {
        for (int i = 0; i < barsBackgrounds.Length; i++)
        {
            barsBackgrounds[i].enabled = i < maxHP ? true : false;
        }
    }


    private IEnumerator Knockback(float xPos)
    {
        PlayerState.instance.isKnockbackedState = true;
        PlayerState.instance.EnableInvulnerability();
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(new Vector2(0.5f * Mathf.Sign(transform.position.x - xPos), 1f) * knockbackForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackTime);
        PlayerState.instance.isKnockbackedState = false;

        PlayerState.instance.DisableInvulnerability(invulnerabilityTime);
    }


    public void TakeDamage(int damage, float xPos)
    {
        currentHP -= damage;
        UpdateBars();

        StartCoroutine(Knockback(xPos));

        if (currentHP < 0)
        {
            Death();
        }
    }
}
