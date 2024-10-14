using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private float movementSpeed;
    private float progress = 0f;
    private bool moveRight = false;

    private float timeSlowedBy = 1f;

    private void Awake() {
        Lever.SlowTimeAction += (int num) => {
            timeSlowedBy = 0.2f;
        };
        Lever.ResetTimeAction += (int num) => {
            timeSlowedBy = 1f;
        };
    }

    private void Update() {
        if (moveRight) {
            progress -= Time.deltaTime * movementSpeed * timeSlowedBy;
        }
        else {
            progress += Time.deltaTime * movementSpeed * timeSlowedBy;
        }
        transform.position = Vector3.Lerp(start.position, end.position, progress);
        progress = Mathf.Clamp(progress, 0f, 1f);
        if (progress <= 0f) {
            moveRight = false;
        }
        else if (progress >= 1f) {
            moveRight = true;
        }
    }
}
