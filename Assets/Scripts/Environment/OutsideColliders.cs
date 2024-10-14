using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideColliders : MonoBehaviour {
    enum ColliderDirection {
        Left,
        Right,
        Up,
        Down, 
        Center
    }

    [SerializeField] private ColliderDirection direction;
    [SerializeField] private CameraSize cameraSize;

    private void Start() {
        switch (direction) {
            case ColliderDirection.Left:    
                transform.position = new Vector3(cameraSize.transform.position.x - (cameraSize.Width / 2), cameraSize.transform.position.y, 0);
                break;
            case ColliderDirection.Right:    
                transform.position = new Vector3(cameraSize.transform.position.x + (cameraSize.Width / 2), cameraSize.transform.position.y, 0);
                break;
            case ColliderDirection.Up:    
                transform.position = new Vector3(cameraSize.transform.position.x , cameraSize.transform.position.y + (cameraSize.Height / 2), 0);
                break;
            case ColliderDirection.Down:    
                transform.position = new Vector3(cameraSize.transform.position.x , cameraSize.transform.position.y - (cameraSize.Height / 2), 0);
                break;
            case ColliderDirection.Center:    
                transform.position = new Vector3(cameraSize.transform.position.x , cameraSize.transform.position.y, 0);
                break;
        }
    }
}
