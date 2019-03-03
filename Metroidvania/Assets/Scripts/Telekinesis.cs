using UnityEngine;

public class Telekinesis : MonoBehaviour
{
    [SerializeField] private float range = 10f;
    [SerializeField] private Transform pickUpCircle = null;

    private Camera _camera = null;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        pickUpCircle.position = _camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
