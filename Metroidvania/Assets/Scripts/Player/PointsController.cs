using UnityEngine;

public class PointsController : MonoBehaviour
{
    [SerializeField] private int containerCapacity = 50;

    private int points = 0;
    private int pointsInContainer = 0;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Point"))
        {
            if (pointsInContainer < containerCapacity)
            {
                pointsInContainer++;
            }
            else
            {
                points++;
            }

            Destroy(collision.gameObject);
        }
    }
}
