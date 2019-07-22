using UnityEngine;
using UnityEngine.UI;

public class PointsController : MonoBehaviour
{
    [SerializeField] private Image container = null;
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
                UpdateContainer();
            }
            else
            {
                points++;
            }

            Destroy(collision.gameObject);
        }
    }


    public void UpdateContainer()
    {
        container.fillAmount = (float)pointsInContainer / containerCapacity;
    }
}
