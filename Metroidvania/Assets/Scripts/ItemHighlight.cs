using UnityEngine;

public class ItemHighlight : MonoBehaviour
{
    [SerializeField] private float minScale = 1f;
    [SerializeField] private float maxScale = 1.1f;
    [SerializeField] private float changeSpeed = 10f;

    private bool wasSmall = true;


    private void Update()
    {
        if ( wasSmall )
        {
            transform.localScale += Vector3.one * Time.deltaTime * changeSpeed;

            if ( transform.localScale.x > maxScale )
            {
                wasSmall = false;
            }
        }
        else
        {
            transform.localScale -= Vector3.one * Time.deltaTime * changeSpeed;

            if ( transform.localScale.x < minScale )
            {
                wasSmall = true;
            }
        }
    }
}
