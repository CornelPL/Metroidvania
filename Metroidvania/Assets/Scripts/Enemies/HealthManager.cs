using UnityEngine;
using UnityEngine.Events;

abstract public class HealthManager : MonoBehaviour
{
    [SerializeField] protected float pointsDropForce = 10f;
    [SerializeField] protected int initialHP = 100;
    [SerializeField] protected GameObject point = null;
    [SerializeField] protected int minPoints = 0;
    [SerializeField] protected int maxPoints = 5;
    [SerializeField] protected SpriteRenderer spriteRenderer = null;
    [SerializeField] protected Color normalColor = Color.white;
    [SerializeField] protected Color damageColor = Color.white;
    [SerializeField] protected float colorChangeTime = 0.5f;
    [SerializeField] protected UnityEvent OnDeath = null;
    [SerializeField] protected UnityEvent OnTakeDamage = null;

    public int currentHP;

    abstract public void TakeDamage(Vector2 direction, int damage);
    virtual public void Knockback(Vector2 direction, float force) { }


    virtual protected void ChangeColorOnDamage()
    {
        LeanTween.value( gameObject, damageColor, normalColor, colorChangeTime ).setOnUpdate( ( Color c ) => { spriteRenderer.color = c; } );
    }


    private void Awake()
    {
        currentHP = initialHP;
    }
}
