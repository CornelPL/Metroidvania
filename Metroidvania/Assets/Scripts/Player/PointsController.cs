using UnityEngine;
using TMPro;

public class PointsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pointsText = null;

    [HideInInspector] public int points = 0;


    public void RemovePoints( int count )
    {
        points -= count;
        UpdatePoints();
    }


    private void Start()
    {
        UpdatePoints();
    }


    private void OnTriggerEnter2D( Collider2D collision )
    {
        if ( collision.CompareTag( "Point" ) )
        {
            points++;
            UpdatePoints();

            Destroy( collision.gameObject );
        }
    }


    private void UpdatePoints()
    {
        pointsText.SetText( points.ToString() );
    }
}