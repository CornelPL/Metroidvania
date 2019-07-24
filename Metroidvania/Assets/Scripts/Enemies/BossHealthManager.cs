using System.Collections;
using UnityEngine;

public class BossHealthManager : HealthManager
{
    public void Death()
    {
        Debug.Log("Boss dead");
        OnDeath.Invoke();
        StartCoroutine(DropPoints());
    }


    private IEnumerator DropPoints()
    {
        int count = Random.Range(minPoints, maxPoints);
        for (int i = 0; i < count; i++)
        {
            Transform inst = Instantiate(point, transform.position, transform.rotation);
            Vector2 dropForce = Random.insideUnitCircle * pointsDropForce;
            dropForce.y = Mathf.Abs(dropForce.y);
            inst.GetComponent<Rigidbody2D>().AddForce(dropForce, ForceMode2D.Impulse);
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }


    override public void TakeDamage(int damage)
    {
        Debug.Log(currentHP);
        currentHP -= damage;
        Debug.Log(currentHP);
    }
}