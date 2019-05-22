using UnityEngine;
using UnityEngine.Events;

public class ItemHandling : MonoBehaviour
{
    public UnityEvent OnStartPulling = null;
    public UnityEvent OnStopPulling = null;
    public UnityEvent OnPullingComplete = null;

    [SerializeField] private ItemPull itemPull = null;
    [SerializeField] private Rigidbody2D _rigidbody = null;


    private void Start()
    {
    }


    public void SetFree()
    {
        transform.SetParent(null);
        _rigidbody.simulated = true;
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
