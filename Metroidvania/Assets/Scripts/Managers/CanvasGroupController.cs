using System.Collections;
using UnityEngine;

public class CanvasGroupController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup = null;
    [SerializeField] private float _fadeTime = 1f;

    private WaitForEndOfFrame _waitTime;
    private float _t = 0f;


    private void Start()
    {
        _waitTime = new WaitForEndOfFrame();
    }


    public void FadeIn()
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        StartCoroutine( nameof( FadeInCoroutine ) );
    }


    public void FadeOut()
    {
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        StartCoroutine( nameof( FadeOutCoroutine ) );
    }


    private IEnumerator FadeInCoroutine()
    {
        while ( _t < _fadeTime )
        {
            _t += Time.unscaledDeltaTime;
            _canvasGroup.alpha = _t;

            yield return _waitTime;
        }

        _t = 0f;
        _canvasGroup.alpha = 1f;
    }


    private IEnumerator FadeOutCoroutine()
    {
        while ( _t < _fadeTime )
        {
            _t += Time.unscaledDeltaTime;
            _canvasGroup.alpha = 1f - (_t / _fadeTime);

            yield return _waitTime;
        }

        _t = 0f;
        _canvasGroup.alpha = 0f;
    }
}
