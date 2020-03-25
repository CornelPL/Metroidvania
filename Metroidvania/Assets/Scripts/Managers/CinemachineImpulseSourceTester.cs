using UnityEngine;

public class CinemachineImpulseSourceTester : MonoBehaviour
{
    [SerializeField, MyBox.MustBeAssigned] private Cinemachine.CinemachineImpulseSource impulseSource = null;


    [MyBox.ButtonMethod]
    private void GenerateImpulse()
    {
        impulseSource.GenerateImpulse();
    }
}
