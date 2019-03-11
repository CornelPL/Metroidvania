using UnityEngine;

public class StableItemHandling : MonoBehaviour
{
    [HideInInspector] public Telekinesis telekinesis = null;

    private Rigidbody2D rbody;
    private float timeToUnstable = 0f;
    private int itemsLayer;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (timeToUnstable >= 0f)
        {
            timeToUnstable -= Time.deltaTime;
        }
        else
        {
            SetUnstable();
        }
    }

    private void SetUnstable()
    {
        rbody.bodyType = RigidbodyType2D.Dynamic;
        rbody.gameObject.layer = itemsLayer;            
        telekinesis.RemoveStableItem();
        Destroy(this);
    }

    public void SetStable(int groundLayer)
    {
        rbody.velocity = Vector2.zero;
        rbody.bodyType = RigidbodyType2D.Kinematic;
        rbody.gameObject.layer = groundLayer;
    }

    public void SetUnstableAfter(float time, int _itemsLayer)
    {
        timeToUnstable = time;
        itemsLayer = _itemsLayer;
    }
}
