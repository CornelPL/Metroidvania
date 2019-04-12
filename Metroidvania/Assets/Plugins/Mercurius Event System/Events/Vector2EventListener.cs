using UnityEngine.Events;
using UnityEngine;

[System.Serializable]
public class UnityEventVector2 : UnityEvent<Vector2> { }
public class Vector2EventListener : EventListenerBase<UnityEventVector2, Vector2Event, Vector2> { }
