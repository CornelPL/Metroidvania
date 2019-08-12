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
    [SerializeField] private float knockbackForce = 100f;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private Collider2D _collider = null;
    [SerializeField] private GameObject destroyEffect = null;
    [SerializeField] private int plankHealth = 3;
    [SerializeField] private List<GameObject> itemsToSpawn = null;
    [SerializeField] private int maxItemsToSpawn = 3;

    private bool isShooted = false;


    private void Awake()
    {
        Assert.IsNotNull(_rigidbody);
        Assert.IsNotNull(_collider);
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isShooted)
        {
            GameObject go = collider.gameObject;
            if (go.CompareTag("Enemy"))
            {
                go.GetComponent<HitManager>().TakeHit(baseDamage, transform.position.x, knockbackForce);
                CustomDestroy(_rigidbody.velocity);
            }
            else if (go.CompareTag("DestroyablePlanks"))
            {
                go.GetComponent<CustomDestroy>().Destroy();
            }

            if (itemType == ItemType.rock)
            {
                CustomDestroy(_rigidbody.velocity);
            }
            else if (itemType == ItemType.crate)
            {
                CustomDestroy(_rigidbody.velocity);
            }
            else if (itemType == ItemType.plank)
            {
                if (go.CompareTag("SoftWall"))
                {
                    plankHealth--;
                    if (plankHealth == 0) CustomDestroy(_rigidbody.velocity);
                    _rigidbody.velocity = Vector2.zero;
                    _rigidbody.bodyType = RigidbodyType2D.Static;
                    gameObject.layer = LayerMask.NameToLayer("PlanksGround");
                    transform.rotation = Quaternion.identity;
                }
                else if (!go.CompareTag("Player"))
                {
                    CustomDestroy(_rigidbody.velocity);
                }
            }
        }
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

        Transform effect = Instantiate( destroyEffect, transform.position, transform.rotation ).transform;
        float angle = Mathf.Atan2( _rigidbody.velocity.y, _rigidbody.velocity.x ) * Mathf.Rad2Deg;
        effect.rotation = Quaternion.AngleAxis( angle, Vector3.forward );
        Destroy( gameObject );
    }


    public void Shoot(Vector2 direction, float power)
    {
        _rigidbody.AddForce(direction * power, ForceMode2D.Impulse);
        _collider.enabled = true;
        _collider.isTrigger = true;
        isShooted = true;
        gameObject.layer = LayerMask.NameToLayer("ShootItem");
    }
}