using UnityEngine;

public class LMBBlinking : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer = null;

    [SerializeField] private float blinkTime = 0.5f;
    [SerializeField] private Vector3 minScale = Vector3.zero;
    [SerializeField] private Vector3 maxScale = Vector3.one;
    [SerializeField] private float minColorAlpha = 0f;
    [SerializeField] private float maxColorAlpha = 1f;

    private Vector3 scaleChange = Vector3.zero;
    private float alphaChange = 0f;
    private float t = 0f;


    private void Start()
    {
        scaleChange = maxScale - minScale;
        alphaChange = maxColorAlpha - minColorAlpha;
    }


    private void Update()
    {
        if ( t < blinkTime )
        {
            float dTime = Time.unscaledDeltaTime;
            float percent = dTime / blinkTime;

            Vector3 scaleUp = transform.localScale + scaleChange * percent;
            transform.localScale = scaleUp;

            Color c = _renderer.color;
            c.a -= alphaChange * percent;
            _renderer.color = c;

            t += dTime;
        }
        else
        {
            SetDefault();
        }
    }


    private void OnDisable()
    {
        SetDefault();
    }


    private void SetDefault()
    {
        transform.localScale = minScale;
        Color rendererColor = _renderer.color;
        _renderer.color = new Color( rendererColor.r, rendererColor.g, rendererColor.b, maxColorAlpha );
        t = 0;
    }
}
