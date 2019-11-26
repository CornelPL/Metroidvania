using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private float xSpeedRatio = 0.0f;
    [SerializeField] private float ySpeedRatio = 0.0f;
    [SerializeField] private bool isParent = false;


    private void Start()
    {
        if ( isParent )
        {
            Parallax.instance.AddElements( transform, xSpeedRatio, ySpeedRatio );
        }
        else
        {
            Parallax.instance.AddElement( transform, xSpeedRatio, ySpeedRatio );
        }
    }


    public void RemoveFromParallax()
    {
        Parallax.instance.RemoveElements(transform);
    }
}
