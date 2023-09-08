using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Signpost : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject signTextBox;
    [SerializeField] private TextMeshProUGUI signText;

    [TextArea(3, 10)]
    [SerializeField] private string[] writtenOnSign;
    [SerializeField] private float textSpeed;

    private int currentSentence;

    private void Awake()
    {
        signText.text = string.Empty;
        currentSentence = 0;
    }

    public void Interact()
    {
        if (signTextBox.activeInHierarchy)
        {
            if(signText.text == writtenOnSign[currentSentence])
            {
                if (currentSentence < writtenOnSign.Length - 1)
                {
                    currentSentence++;
                    signText.text = string.Empty;
                    StartCoroutine(TypeLine());
                }
                else
                    signTextBox.SetActive(false);
            }
            else
            {
                StopAllCoroutines();
                signText.text = writtenOnSign[currentSentence];
            }

        }
        else
        {
            currentSentence = 0;
            signTextBox.SetActive(true);
            signText.text = string.Empty;
            StartCoroutine(TypeLine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
            signTextBox.SetActive(false);
    }

    IEnumerator TypeLine()
    {
        foreach (char c in writtenOnSign[currentSentence].ToCharArray())
        {
            signText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
