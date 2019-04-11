using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToInstantiate = null;
    [SerializeField] private int nMinObjects = 2;
    [SerializeField] private int nMaxObjects = 5;
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 15f;

    public void CustomDestroy()
    {
        // explosion particles

        int nOfObjects = Random.Range(nMinObjects, nMaxObjects);
        for (int i = 0; i < nOfObjects; i++)
        {
            int randomObject = Random.Range(0, objectsToInstantiate.Length);
            GameObject objectToInstantiate = objectsToInstantiate[randomObject];
            GameObject instantiated = Instantiate(objectToInstantiate, transform.position, Quaternion.identity);

            float force = Random.Range(minForce, maxForce);
            Vector2 direction = Random.insideUnitCircle;
            if (direction.y < 0f) direction.y = -direction.y;
            direction.Normalize();
            direction = transform.TransformDirection(direction);

            instantiated.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        DrawArrow.ForGizmo(transform.position, transform.up * 2f, Color.red);
    }
}
