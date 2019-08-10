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
        Parallax.instance.AddElement(transform, xSpeedRatio, ySpeedRatio);
    }


    public void RemoveFromParallax()
    {
        Parallax.instance.RemoveElement(transform);
    }
}
