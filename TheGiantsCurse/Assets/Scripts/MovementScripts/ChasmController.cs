using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasmController : PlayerController
{
    [SerializeField] private float runningSpeed = 10f;
    [SerializeField] private float runDelay = 5f;

    private bool isRunning = false;

}
