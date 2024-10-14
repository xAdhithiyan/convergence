using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftBase : MonoBehaviour
{
    [SerializeField] private CameraSize cameraSize; 
    
    private void Start()
    {
        setDimensions();
    }

    private void setDimensions()
    {
        float x = cameraSize.Width / 4;
        
        transform.position = new Vector3(cameraSize.transform.position.x - x, cameraSize.transform.position.y , 0);
        transform.localScale = new Vector3( cameraSize.Width / 2, cameraSize.Height, 0);
    }
}
