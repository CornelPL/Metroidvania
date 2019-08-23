using UnityEngine;
using UnityEngine.Events;

abstract public class HealthManager : MonoBehaviour
{
    protected float pointsDropForce = 10f;

    [SerializeField] protected int initialHP = 100;
    [SerializeField] protected Transform point = null;
    [SerializeField] protected int minPoints = 0;
    [SerializeField] protected int maxPoints = 5;
    [SerializeField] protected SpriteRenderer spriteRenderer = null;
    [SerializeField] protected Color normalColor = Color.white;
    [SerializeField] protected Color damageColor = Color.white;
    [SerializeField] protected float colorChangeTime = 0.5f;
    [SerializeField] protected UnityEvent OnDeath = null;
    [SerializeField] protected UnityEvent OnTakeDamage = null;

    public int currentHP;

    abstract public void TakeDamage(int damage);
    virtual public void Knockback(float xPosition, float force) { }


    virtual public void ChangeColorOnDamage()
    {
        LeanTween.value( gameObject, damageColor, normalColor, colorChangeTime ).setOnUpdate( ( Color c ) => { spriteRenderer.color = c; } );
    }


    private void Awake()
    {
        currentHP = initialHP;
    }
}
