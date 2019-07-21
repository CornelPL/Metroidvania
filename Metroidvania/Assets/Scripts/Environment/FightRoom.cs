using UnityEngine;

public class FightRoom : MonoBehaviour
{
    [SerializeField] private Door[] doors = null;
    [SerializeField] private FightTrigger[] fightTriggers = null;
    [SerializeField] private int enemiesCount = 5;


    public void RemoveEnemy()
    {
        enemiesCount--;
        if (enemiesCount == 0)
        {
            SetFightRoomPassed();
        }
    }


    public void CloseRoom()
    {
        foreach(Door door in doors)
        {
            door.Close();
        }
        SetFightTriggersActive(false);
    }


    private void OpenRoom()
    {
        foreach (Door door in doors)
        {
            door.Open();
        }
    }


    private void SetFightTriggersActive(bool b)
    {
        foreach(FightTrigger trigger in fightTriggers)
        {
            trigger.gameObject.SetActive(b);
        }
    }


    private void SetFightRoomPassed()
    {
        OpenRoom();
        SetFightTriggersActive(false);
    }
}
