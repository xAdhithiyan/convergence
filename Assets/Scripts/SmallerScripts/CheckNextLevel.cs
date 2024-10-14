using System;
using UnityEngine;

public class CheckNextLevel : MonoBehaviour
{
    public LeftCheck leftCollider;
    public RightCheck rightCollider;

    private bool updateNextLevel = false;
    
    private void Update() {
        if (leftCollider.leftCheck && rightCollider.rightCheck && !updateNextLevel) {
            GameManager.Instance.LoadNextLevel();
            updateNextLevel = true;
        }
    }
}