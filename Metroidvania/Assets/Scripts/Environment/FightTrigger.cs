using UnityEngine;

public class FightTrigger : MonoBehaviour
{
    [SerializeField] private FightRoom fightRoom = null;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            fightRoom.CloseRoom();
        }
    }
}
