using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private GameObject gravityProjectile = null;
    [SerializeField] private Transform startPos = null;
    [SerializeField] private float shotPower = 10f;
    [SerializeField] private float waitTime = 2f;

    private InputController input;
    private PlayerState state;
    private bool canShoot = true;

    void Start()
    {
        input = InputController.instance;
        state = PlayerState.instance;
    }

    void Update()
    {
        if (state.hasExplosion && input.lmbDown && !state.isHoldingItemState && !state.isPullingItemState && canShoot)
        {
            GameObject projectile = Instantiate(gravityProjectile, startPos.position, Quaternion.identity);
            Vector2 direction = input.cursorPosition - (Vector2)startPos.position;
            direction.Normalize();
            projectile.GetComponent<Rigidbody2D>().AddForce(direction * shotPower, ForceMode2D.Impulse);
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        canShoot = false;
        yield return new WaitForSeconds(waitTime);
        canShoot = true;
    }
}
