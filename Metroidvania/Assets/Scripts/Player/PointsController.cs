using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PointsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText = null;

    private int points = 0;


    private void Start()
    {
        UpdatePoints();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Point"))
        {
            points++;
            UpdatePoints();

            Destroy(collision.gameObject);
        }
    }


    private void UpdatePoints()
    {
        pointsText.SetText(points.ToString());
    }
}
