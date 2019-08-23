using UnityEngine;
using UnityEngine.Events;

abstract public class HealthManager : MonoBehaviour
{
    protected float pointsDropForce = 10f;

    [SerializeField] protected int initialHP = 100;
    [SerializeField] protected Transform point = null;
    [SerializeField] protected int minPoints = 0;
    [SerializeField] protected int maxPoints = 5;
    [SerializeField] protected UnityEvent OnDeath = null;
    [SerializeField] protected UnityEvent OnTakeDamage = null;

    public int currentHP;

    abstract public void TakeDamage(int damage);
    virtual public void Knockback(float xPosition, float force) { }

    private void Awake()
    {
        currentHP = initialHP;
    }
}
