using UnityEngine;
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
    [SerializeField] private Rigidbody2D rb = null;
    public bool isShooted = false;
    [SerializeField] private int plankHealth = 3;
    [SerializeField] private List<GameObject> itemsToSpawn = null;
    [SerializeField] private int maxItemsToSpawn = 3;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isShooted)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                DoDamage(collision.gameObject);
                CustomDestroy();
            }

            if (itemType == ItemType.rock)
            {
                CustomDestroy();
            }
            else if (itemType == ItemType.crate)
            {
                CustomDestroy();
            }
            else if (itemType == ItemType.plank)
            {
                if (collision.gameObject.CompareTag("SoftWall"))
                {
                    plankHealth--;
                    if (plankHealth == 0) CustomDestroy();
                    rb.velocity = Vector2.zero;
                    rb.bodyType = RigidbodyType2D.Static;
                    gameObject.layer = LayerMask.NameToLayer("PlanksGround");
                    transform.rotation = Quaternion.identity;
                }
                else if (!collision.gameObject.CompareTag("Player"))
                {
                    CustomDestroy();
                }
            }
        }
    }


    private void DoDamage(GameObject go)
    {
        int damage = (int)(baseDamage * rb.velocity.magnitude / 10 * rb.mass);
        go.GetComponent<EnemyHealthManager>().TakeDamage(damage, transform.position.x);
    }


    private void CustomDestroy()
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
                inst.GetComponent<Rigidbody2D>().velocity = rb.velocity;
            }
        }

        Destroy(gameObject);
    }
}