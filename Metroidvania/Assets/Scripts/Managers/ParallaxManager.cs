using UnityEngine;

public class ParallaxManager : MonoBehaviour
{
    [SerializeField] private float xSpeedRatio = 0.0f;
    [SerializeField] private float ySpeedRatio = 0.0f;


    private void Start()
    {
        AddToParallax();
    }

    
    public void AddToParallax()
    {
        Parallax.instance.AddElements(transform, xSpeedRatio, ySpeedRatio);
    }


    public void RemoveFromParallax()
    {
        Parallax.instance.RemoveElements(transform);
    }
}
