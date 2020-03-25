using UnityEngine;
using Cinemachine;
using MyBox;

public class DeathEffectController : MonoBehaviour
{
    [SerializeField, MustBeAssigned] private CinemachineImpulseSource impulseSource = null;


    private void Start()
    {
        impulseSource.GenerateImpulse();
    }
}
