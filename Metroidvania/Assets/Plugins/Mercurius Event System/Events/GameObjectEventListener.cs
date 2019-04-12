using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class UnityEventGameObject : UnityEvent<GameObject> { }
public class GameObjectEventListener : EventListenerBase<UnityEventGameObject, GameObjectEvent, GameObject> { }
