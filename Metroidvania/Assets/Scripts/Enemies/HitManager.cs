using UnityEngine;
using UnityEngine.Events;

public class HitManager : MonoBehaviour
{
    [SerializeField] private bool canBeKnockbacked = false;
    [Tooltip("Leave null to don't take damage")]
    [SerializeField] private HealthManager healthManager = null;
    [SerializeField] private UnityEvent OnHit = null;


    public void TakeHit(int damage, float xPosition)
    {
        healthManager?.TakeDamage(damage);
        if (canBeKnockbacked) healthManager.Knockback(xPosition);
        OnHit.Invoke();
    }
}
