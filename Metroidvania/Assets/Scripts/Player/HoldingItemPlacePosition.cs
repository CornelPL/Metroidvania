using UnityEngine;


public class HoldingItemPlacePosition : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private float smoothSpeed = 0.2f;
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private float knockbackFade = 0.9f;
    [SerializeField] private float randomOffsetWeight = 2f;
    [SerializeField] private float offsetToCursorWeight = 1.5f;

    private Vector2 offset = Vector2.zero;
    private Vector2 randomOffset = Vector2.zero;
    private Vector2 knockbackVelocity = Vector2.zero;
    private InputController input = null;


    public void Knockback( Vector2 direction )
    {
        knockbackVelocity = direction * knockbackForce;
    }


    private void Start()
    {
        input = InputController.instance;
    }


    private void Update()
    {
        SetOffsetToCursor();
        SetRandomOffset();
        UpdatePosition();
    }


    private void SetOffsetToCursor()
    {
        offset = input.cursorPosition - (Vector2)target.position;
        offset.Normalize();

        offset *= offsetToCursorWeight;
    }


    private void SetRandomOffset()
    {
        randomOffset.x = Mathf.PerlinNoise( Time.time, 0f ) - 0.5f;
        randomOffset.y = Mathf.PerlinNoise( 0f, Time.time ) - 0.5f;

        randomOffset *= randomOffsetWeight;
    }


    private void UpdatePosition()
    {
        knockbackVelocity *= knockbackFade;

        float desireX = Mathf.SmoothStep( transform.position.x, target.position.x + offset.x + randomOffset.x, smoothSpeed );
        float desireY = Mathf.SmoothStep( transform.position.y, target.position.y + offset.y + randomOffset.y, smoothSpeed );
        desireX += knockbackVelocity.x;
        desireY += knockbackVelocity.y;

        transform.position = new Vector3( desireX, desireY );
    }
}
