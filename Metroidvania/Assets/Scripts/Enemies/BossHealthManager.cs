using System.Collections;
using UnityEngine;

public class BossHealthManager : HealthManager
{
    [SerializeField] private float normalBrightness = 0.2f;
    [SerializeField] private float hitBrightness = 1f;
    [SerializeField] private float brightnessChangeTime = 0.5f;


    override public void ChangeColorOnDamage()
    {
        LeanTween.value( gameObject, hitBrightness, normalBrightness, brightnessChangeTime ).setOnUpdate( ( float v ) => { spriteRenderer.material.SetFloat( "_Brightness", v ); } );
    }


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
        currentHP -= damage;
        OnTakeDamage.Invoke();
    }
}