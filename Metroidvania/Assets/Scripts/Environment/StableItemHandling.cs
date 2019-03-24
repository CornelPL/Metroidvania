using UnityEngine;
using System.Collections;

public class StableItemHandling : MonoBehaviour
{
    [HideInInspector] public Telekinesis telekinesis = null;

    private float timeToUnstable = 0f;
    private int itemsLayer;
    private int groundLayer;
    private Coroutine coroutine;
    private bool isCoroutineRunning = false;

    [SerializeField] private BoxCollider2D _collider = null;
    [SerializeField] private SpriteRenderer _renderer = null;
    [SerializeField] private Color unstableColor = Color.white;
    [SerializeField] private Color stableColor = Color.white;
    [SerializeField] private string itemsLayerS = "Items";
    [SerializeField] private string groundLayerS = "Ground";

    private void Start()
    {
        groundLayer = LayerMask.NameToLayer(groundLayerS);
        itemsLayer = LayerMask.NameToLayer(itemsLayerS);
    }

    private void SetUnstable()
    {
        // Disappear animation
        _renderer.color = unstableColor;
        _collider.isTrigger = true;
        telekinesis.RemoveStableItem();
        gameObject.layer = itemsLayer;
        if (isCoroutineRunning)
        {
            StopCoroutine(coroutine);
            isCoroutineRunning = false;
        }
    }

    private IEnumerator Timer()
    {
        isCoroutineRunning = true;

        while (timeToUnstable >= 0f)
        {
            timeToUnstable -= Time.deltaTime;
            yield return null;
        }

        SetUnstable();

        isCoroutineRunning = false;
    }

    public void SetStable()
    {
        // Appear animation
        _renderer.color = stableColor;
        _collider.isTrigger = false;
        gameObject.layer = groundLayer;
    }

    public void SetUnstable(float time)
    {
        timeToUnstable = time;
        coroutine = StartCoroutine(Timer());
    }
}
