using UnityEngine;

public class ItemHandling : MonoBehaviour
{
    [SerializeField] private ItemPull itemPull = null;
    [SerializeField] private ParticleSystem particles = null;
    [SerializeField] private ParticleSystem particles2 = null;


    private void Start()
    {
        particles.Stop();
        particles2.Stop();
    }


    public void PullItem(Transform t, float s, float ms)
    {
        itemPull.enabled = true;
        itemPull.Pull(t, s, ms);
    }


    public void StopPulling()
    {
        itemPull.StopPulling();
    }
}
