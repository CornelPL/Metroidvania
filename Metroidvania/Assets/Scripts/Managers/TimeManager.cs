using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance = null;

    [SerializeField] private float slowmoTimeScale = 0.1f;
    [SerializeField] private float timeChangeDecreaseSpeed = 2f;
    [SerializeField] private float timeChangeIncreaseSpeed = 5f;

    private IEnumerator runningCoroutine = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(this);
    }

    public void TurnSlowmoOn()
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);
        runningCoroutine = Decrease();
        StartCoroutine(runningCoroutine);
    }

    public void TurnSlowmoOff()
    {
        if (runningCoroutine != null)
            StopCoroutine(runningCoroutine);
        runningCoroutine = Increase();
        StartCoroutine(runningCoroutine);
    }

    private IEnumerator Decrease()
    {
        while (Time.timeScale > slowmoTimeScale)
        {
            Time.timeScale -= timeChangeDecreaseSpeed * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return null;
        }

        Time.timeScale = slowmoTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        StopCoroutine(runningCoroutine);
    }

    private IEnumerator Increase()
    {
        while (Time.timeScale < 1f)
        {
            Time.timeScale += timeChangeIncreaseSpeed * Time.unscaledDeltaTime;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        StopCoroutine(runningCoroutine);
    }
}
