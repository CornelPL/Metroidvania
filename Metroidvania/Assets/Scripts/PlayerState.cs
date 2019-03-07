using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState instance = null;

    [HideInInspector] public bool isHoldingItemState = false;
    [HideInInspector] public bool isPullingItemState = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }
}
