using UnityEngine;

public class RotateCape : MonoBehaviour
{
    [SerializeField] private ClothSim2D cape = null;


    public void Rotate( Vector2 sourcePosition )
    {
        Vector2 windDirection = (Vector2)transform.position - sourcePosition;

        float windAngle = Mathf.Atan2( windDirection.y, windDirection.x ) * Mathf.Rad2Deg + 90f;
        windAngle = windAngle > 180f ? windAngle - 360f : windAngle;

        float currentAngle = transform.rotation.eulerAngles.z;
        currentAngle = currentAngle > 180f ? currentAngle - 360f : currentAngle;

        if ( (currentAngle >= 0f && currentAngle < windAngle) || 
             (currentAngle < 0f && currentAngle > windAngle) )
        {
            transform.eulerAngles = new Vector3( 0f, 0f, windAngle );
        }

        cape.Recalculate();
    }
}