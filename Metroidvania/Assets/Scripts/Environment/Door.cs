using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private ParticleSystem movingParticles = null;
    [SerializeField] private GameObject onCloseEffect = null;


    public void Open()
    {
        animator.SetTrigger( "open" );
    }


    public void Close()
    {
        animator.SetTrigger( "close" );
    }


    public void PlayMovingParticles()
    {
        if ( !movingParticles.isPlaying )
        {
            movingParticles.Play();
        }
    }


    public void StopMovingParticles()
    {
        if ( movingParticles.isPlaying )
        {
            movingParticles.Stop();
        }
    }


    public void OnCloseEffect()
    {
        onCloseEffect.SetActive( true );
    }
}
