using UnityEngine;

public class christmastree : MonoBehaviour
{
    [SerializeField] private ParticleSystem treeParticles = null;
    [SerializeField] private ParticleSystem firstExplosion = null;
    [SerializeField] private GameObject[] balls = null;

    int i = 0;

    private void SpawnFirstExplosion()
    {
        treeParticles.Stop();
        firstExplosion.Play();
        for ( i = 0; i < balls.Length; i++ )
        {
            Invoke( "SpawnBall", i * 0.5f );
        }
        i = 0;
    }

    private void SpawnBall()
    {
        balls[ i++ ].SetActive( true );
    }
}
