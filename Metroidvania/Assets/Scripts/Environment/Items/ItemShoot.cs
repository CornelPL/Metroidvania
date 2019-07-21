using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class ItemShoot : MonoBehaviour
{
    public enum ItemType
    {
        rock,
        crate,
        plank,
        barrel,
        saw
    }

    public ItemType itemType = ItemType.rock;
    [SerializeField] private int baseDamage = 10;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private Collider2D _collider = null;
    [SerializeField] private int plankHealth = 3;
    [SerializeField] private List<GameObject> itemsToSpawn = null;
    [SerializeField] private int maxItemsToSpawn = 3;

    private bool isShooted = false;


    private void Awake()
    {
        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_collider);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isShooted)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                DoDamage(collision.gameObject);
                CustomDestroy(collision.relativeVelocity);
            }
            else if (collision.gameObject.CompareTag("DestroyablePlanks"))
            {
                collision.gameObject.GetComponent<CustomDestroy>().Destroy();
            }

            if (itemType == ItemType.rock)
            {
                CustomDestroy(collision.relativeVelocity);
            }
            else if (itemType == ItemType.crate)
            {
                CustomDestroy(collision.relativeVelocity);
            }
            else if (itemType == ItemType.plank)
            {
                if (collision.gameObject.CompareTag("SoftWall"))
                {
                    plankHealth--;
                    if (plankHealth == 0) CustomDestroy(collision.relativeVelocity);
                    _rigidbody.velocity = Vector2.zero;
                    _rigidbody.bodyType = RigidbodyType2D.Static;
                    gameObject.layer = LayerMask.NameToLayer("PlanksGround");
                    transform.rotation = Quaternion.identity;
                }
                else if (!collision.gameObject.CompareTag("Player"))
                {
                    CustomDestroy(collision.relativeVelocity);
                }
            }
        }
    }


    private void DoDamage(GameObject go)
    {
        int damage = (int)(baseDamage * _rigidbody.velocity.magnitude / 10 * _rigidbody.mass);
        go.GetComponent<EnemyHealthManager>().TakeDamage(damage, transform.position.x);
    }


    private void CustomDestroy(Vector2 collisionVelocity)
    {
        if (itemType == ItemType.crate)
        {
            int item = Random.Range(0, itemsToSpawn.Count);
            int i = Random.Range(1, maxItemsToSpawn);

            for(int a = 0; a < i; a++)
            {
                Vector3 randomRotation = transform.eulerAngles;
                randomRotation.z = Random.Range(0f, 360f);
                GameObject inst = Instantiate(itemsToSpawn[item], transform.position + (Vector3)Random.insideUnitCircle, transform.rotation);
                inst.transform.eulerAngles = randomRotation;
                inst.GetComponent<Rigidbody2D>().velocity = -collisionVelocity;
            }
        }

        Destroy(gameObject);
    }


    public void Shoot(Vector2 direction, float power)
    {
        _rigidbody.AddForce(direction * power, ForceMode2D.Impulse);
        _collider.enabled = true;
        isShooted = true;
    }
}