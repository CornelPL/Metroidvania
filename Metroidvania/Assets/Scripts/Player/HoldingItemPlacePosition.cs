using UnityEngine;

public class HoldingItemPlacePosition : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float smoothSpeed = 0.2f;
    [SerializeField] private float range = 2f;
    [SerializeField] private Vector2 offset = Vector2.zero;

    private Vector2 randomOffset = Vector2.zero;


    private void Update()
    {
        if (PlayerState.instance.isPullingItemState) return;

        ChooseOffset();

        UpdateCameraPosition();
    }


    private void ChooseOffset()
    {
        randomOffset.x = Mathf.PerlinNoise(Time.time, 0f) - 0.5f;
        randomOffset.y = Mathf.PerlinNoise(0f, Time.time) - 0.5f;

        randomOffset *= range;
    }


    private void UpdateCameraPosition()
    {
        float desireX = Mathf.SmoothStep(transform.position.x, target.position.x + randomOffset.x + offset.x, smoothSpeed);
        float desireY = Mathf.SmoothStep(transform.position.y, target.position.y + randomOffset.y + offset.y, smoothSpeed);

        transform.position = new Vector2(desireX, desireY);
    }
}
