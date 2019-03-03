/**
 * Description: Adds ability to perform interactions by player on an object
 * Authors: Wojciech Bruski, Rebel Game Studio
 * Copyright: © 2018-2019 Rebel Game Studio. All rights reserved. For license see: 'LICENSE' file.
 **/

using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    [SerializeField, Tooltip("Interaction button")] private KeyCode key = KeyCode.E;

    [SerializeField] private UnityEvent onInteract = null;

    public void Interact()
    {
        if (Input.GetKeyDown(key))
        {
            onInteract.Invoke();
        }
    }
}
