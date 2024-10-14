using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSize : MonoBehaviour {
    public float Height { get; private set; }
    public float Width { get; private set; }
    
    private void Awake() {
        Height = Camera.main.orthographicSize * 2;
        Width = Height * Screen.width / Screen.height;
    }
}
