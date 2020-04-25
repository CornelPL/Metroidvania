using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource = null;
    [SerializeField] private AudioClip _inGameMusic = null;
    [SerializeField] private AudioClip _bossMusic = null;
    [SerializeField] private float _fadeTime = 3f;

    private WaitForEndOfFrame _wait;
    private float _fadeOutT = 0f;
    private float _fadeInT = 0f;
    private float _volume;


    public void StartInGameMusic()
    {
        StartCoroutine( FadeOutVolume() );
        Invoke( nameof( PlayInGameMusic ), _fadeTime );
    }


    public void StartBossMusic()
    {
        StartCoroutine( FadeOutVolume() );
        Invoke( nameof( PlayBossMusic ), _fadeTime );
    }


    private void Start()
    {
        _wait = new WaitForEndOfFrame();
        _volume = _audioSource.volume;
    }


    private void PlayInGameMusic()
    {
        if ( _audioSource.isPlaying )
        {
            _audioSource.Stop();
        }

        _audioSource.clip = _inGameMusic;
        _audioSource.Play();

        StartCoroutine( FadeInVolume() );
        Invoke( nameof( StartInGameMusic ), _inGameMusic.length - _fadeTime * 2 );
    }


    private void PlayBossMusic()
    {
        if ( _audioSource.isPlaying )
        {
            _audioSource.Stop();
        }

        _audioSource.clip = _bossMusic;
        _audioSource.Play();

        StartCoroutine( FadeInVolume() );
        Invoke( nameof( StartBossMusic ), _bossMusic.length - _fadeTime * 2 );
    }


    private IEnumerator FadeOutVolume()
    {
        while ( _fadeOutT < _fadeTime )
        {
            _fadeOutT += Time.unscaledDeltaTime;
            _audioSource.volume = _volume * (1f - (_fadeOutT / _fadeTime));

            yield return _wait;
        }

        _fadeOutT = 0f;
        _audioSource.volume = 0f;
    }


    private IEnumerator FadeInVolume()
    {
        while ( _fadeInT < _fadeTime )
        {
            _fadeInT += Time.unscaledDeltaTime;
            _audioSource.volume = _volume * (_fadeInT / _fadeTime);

            yield return _wait;
        }

        _fadeInT = 0f;
        _audioSource.volume = _volume;
    }
}
