using UnityEngine;


public class HoldingItemPlacePosition : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float smoothSpeed = 0.2f;
    [SerializeField] private float range = 2f;
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float knockbackFade = 0.9f;

    private Vector2 randomOffset = Vector2.zero;
    private Vector2 knockbackVelocity = Vector2.zero;


    public void Knockback( Vector2 direction )
    {
        knockbackVelocity = direction * knockbackForce;
    }


    private void Update()
    {
        ChooseOffset();

        UpdatePosition();
    }


    private void ChooseOffset()
    {
        randomOffset.x = Mathf.PerlinNoise( Time.time, 0f ) - 0.5f;
        randomOffset.y = Mathf.PerlinNoise( 0f, Time.time ) - 0.5f;

        randomOffset *= range;
    }


    private void UpdatePosition()
    {
        knockbackVelocity *= knockbackFade;

        float desireX = Mathf.SmoothStep( transform.position.x, target.position.x + randomOffset.x, smoothSpeed );
        float desireY = Mathf.SmoothStep( transform.position.y, target.position.y + randomOffset.y, smoothSpeed );
        desireX += knockbackVelocity.x;
        desireY += knockbackVelocity.y;

        transform.position = new Vector3( desireX, desireY );
    }
}
