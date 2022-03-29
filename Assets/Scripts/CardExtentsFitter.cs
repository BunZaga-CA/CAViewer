using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardExtentsFitter : MonoBehaviour
{
    [SerializeField] private Camera currentCamera;
    [SerializeField] private float targetWidth;
    [SerializeField] private float targetHeight;
    [SerializeField] private float targetSize;
    private void Awake()
    {
        float screenRatio = ((float) Screen.width / (float)Screen.height);
        float targetRatio =  targetWidth / targetHeight;

        if (screenRatio > targetRatio)
        {
            currentCamera.orthographicSize = targetSize / 2;
        }
        else
        {
            float difference = targetRatio / screenRatio;
            currentCamera.orthographicSize = (targetSize / 2) * difference;
        }
    }
}
