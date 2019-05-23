using UnityEngine;
using UnityEngine.Events;

public class ItemHandling : MonoBehaviour
{
    public UnityEvent OnStartPulling = null;
    public UnityEvent OnPullingComplete = null;
    public UnityEvent OnRelease = null;

    [SerializeField] private ItemPull itemPull = null;
    [SerializeField] private Rigidbody2D _rigidbody = null;


    public void SetFree()
    {
        transform.SetParent(null);
        _rigidbody.simulated = true;
        OnRelease.Invoke();
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
