using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArrowCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI arrowNumber;
    [SerializeField] private GameObject ropeImage;
    
    public void UpdateArrowNumber(int number)
    {
        arrowNumber.text = number.ToString();
    }

    public void SetRopeImage(bool active)
    {
        ropeImage.SetActive(active);
    }
}
