using UnityEngine;

public class FallingSpikes : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float gravityScale = 8f;
    [SerializeField] private float fallDelay = 1f;
    [SerializeField] private Rigidbody2D _rigidbody = null;
    [SerializeField] private GameObject fallParticles = null;

    private bool isFalling = false;


    public void StartFalling()
    {
        isFalling = true;
        Instantiate( fallParticles, transform.position, Quaternion.identity, null );
        Invoke( nameof( DoFall ), fallDelay );
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isFalling)
        {
            StartFalling();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealthManager>().TakeDamage(damage, transform.position.x);
        }

        Destroy(gameObject);
    }


    private void DoFall()
    {
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody.gravityScale = gravityScale;
    }
}
