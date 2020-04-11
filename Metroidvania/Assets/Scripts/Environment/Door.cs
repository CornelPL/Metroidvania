using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    [SerializeField] private ParticleSystem movingParticles = null;
    [SerializeField] private GameObject onCloseEffect = null;
    [SerializeField] private bool openOnStart = true;


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


    private void OnEnable()
    {
        if ( openOnStart )
            animator.SetTrigger( "open" );
        else
            animator.SetTrigger( "close" );
    }
}
