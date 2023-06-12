using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleTransition : MonoBehaviour
{
    private Canvas canvas;
    private Image blackScreen;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        blackScreen = GetComponentInChildren<Image>();
    }

    private void Start()
    {
        DrawBlackScreen();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OpenBlackScreen();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CloseBlackScreen();
        }
    }

    private void DrawBlackScreen()
    {
        var canvasRect = canvas.GetComponent<RectTransform>().rect;
        var canvasWidth = canvasRect.width;
        var canvasHeight = canvasRect.height;

        var squareValue = 0f;
        if (canvasWidth > canvasHeight) 
        {
            squareValue = canvasWidth;
        }
        else
        {
            squareValue = canvasHeight;
        }

        blackScreen.rectTransform.sizeDelta = new Vector2(squareValue, squareValue);
    }

    public void OpenBlackScreen()
    {
        StartCoroutine(Transition(1, 0, 1));
    }

    public void CloseBlackScreen()
    {
        StartCoroutine(Transition(1, 1, 0));
    }

    private IEnumerator Transition(float duration, float begin, float goal)
    {
        var time = 0f;
        while (time <= duration)
        {
            time += Time.deltaTime;
            var t = time / duration;
            var radius = Mathf.Lerp(begin, goal, t);

            blackScreen.material.SetFloat("_Radius", radius);

            yield return null;
        }
    }
}
