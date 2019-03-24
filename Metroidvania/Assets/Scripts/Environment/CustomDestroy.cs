using UnityEngine;
using UnityEngine.Events;

public class CustomDestroy : MonoBehaviour
{
    [SerializeField] private UnityEvent OnDestroy = null;

    public void Destroy()
    {
        OnDestroy.Invoke();
    }
}
