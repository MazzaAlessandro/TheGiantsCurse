using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionHandler : MonoBehaviour
{
    public static TransitionHandler instance;

    [SerializeField] private CircleTransition circleTransition;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void CloseAndOpen(float duration)
    {
        StartCoroutine(OCCoroutine(duration));
    }

    private IEnumerator OCCoroutine(float duration)
    {
        circleTransition.CloseBlackScreen();
        yield return new WaitForSeconds(duration);
        circleTransition.OpenBlackScreen();
    }

    public void Open()
    {
        circleTransition.OpenBlackScreen();
    }
}
