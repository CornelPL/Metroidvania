using UnityEngine;
using UnityEditor;

public class ExitButton : MonoBehaviour
{
    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif

#if UNITY_STANDALONE
        Application.Quit();
#endif
    }
}
