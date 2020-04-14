using UnityEngine;

public class EasterSpecial : MonoBehaviour
{
    [SerializeField] private GameObject[] eggs = null;
    [SerializeField] private TMPro.TMP_Text text = null;
    [SerializeField] private float fadeSpeed = 2f;

    private bool showFont = false;


    private void Update()
    {
        if ( Input.GetKey( KeyCode.N ) || Input.GetKeyDown( KeyCode.B ) )
        {
            SpawnEgg();
        }

        if ( Input.GetKey( KeyCode.V ) )
        {
            showFont = true;
        }

        if ( showFont )
        {
            float alpha = text.color.a + Time.deltaTime * fadeSpeed;
            text.color = new Color( 1f, 0f, 0f, alpha );
        }
    }


    private void SpawnEgg()
    {
        GameObject egg = eggs[ Random.Range( 0, eggs.Length ) ];
        Quaternion randomRotation = Quaternion.AngleAxis( Random.Range( 0f, 360f ), Vector3.forward );
        Instantiate( egg, transform.position, randomRotation );
    }
}
