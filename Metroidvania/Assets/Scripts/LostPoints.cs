using UnityEngine;

public class LostPoints : MonoBehaviour
{
    [SerializeField] private float frequency = 1f;
    [SerializeField] private float amplitude = 1f;

    [HideInInspector] public int points = 0;

    private Vector2 spawnPos;
    private float t = 0f;


    private void Start()
    {
        spawnPos = transform.position;
    }


    private void Update()
    {
        t += Time.deltaTime;
        transform.position = new Vector2( spawnPos.x, spawnPos.y + amplitude * Mathf.Sin( t * frequency ) );
    }


    private void OnTriggerEnter2D( Collider2D collider )
    {
        PointsController.instance.AddPoints( points );
        // TODO: Play destroy particles
        Destroy( gameObject );
    }
}
