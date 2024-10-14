using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using Unity.VisualScripting;
using UnityEngine;

public class Lever : MonoBehaviour
{
    [SerializeField] private float checkPlayerRadius;
    public enum Functionality {
        MovePlatform,
        SlowTime
    }

    private enum Direction {
        Left, 
        Right,
    }

    private enum CurrentState
    {
        On,
        Off,
    }

    [SerializeField] public Functionality functionality;
    [SerializeField] private Direction direction;
    [SerializeField] private Transform platformTransform;
    private CurrentState currentState = CurrentState.Off; 
    
    // Platform Moving Data
    private float progess = 0f; 
    private bool canPlatformMove = false;
    private Vector2 start;
    private Vector2 end;
    
    // Slowing time Data
    public static event Action<int> SlowTimeAction;
    public static event Action<int> ResetTimeAction;


    private void Start() {
        if (functionality == Functionality.MovePlatform) {
            if (direction == Direction.Left) {
                start = platformTransform.position;
                end = new Vector2(platformTransform.position.x + platformTransform.localScale.x , platformTransform.position.y);
            } else if (direction == Direction.Right) {
                start = platformTransform.position;
                end = new Vector2(platformTransform.position.x - platformTransform.localScale.x,
                    platformTransform.position.y);
            }
        }
    }
    
    private bool CheckForLever() {
        Collider2D[] playerPresent = Physics2D.OverlapCircleAll(transform.position, checkPlayerRadius);
        foreach (var item in playerPresent) {
            if (item.gameObject.CompareTag("Player")) {
                return true;
            }
        }
        return false;
    }
    
    public void ActivateLever(int playerNumber) {
        FindObjectOfType<AudioManager>().Play("lever");

        if (CheckForLever()) { 
            currentState = currentState == CurrentState.Off ? CurrentState.On : CurrentState.Off;
            switch (functionality)  {
                case Functionality.MovePlatform:
                    canPlatformMove = true;
                    break;
                case Functionality.SlowTime:
                    SlowTime(playerNumber);
                    break;
             }
        }
    }

    private void Update() {
        if (canPlatformMove) {
            MovePlatform();
        }
    }

    private void MovePlatform() {
        
        platformTransform.position = Vector2.Lerp(start, end, progess);

        if (currentState == CurrentState.On) {
            progess += Time.deltaTime;
        } else if (currentState == CurrentState.Off) {
            progess -= Time.deltaTime;
        }
        
        progess = Mathf.Clamp(progess, 0f, 1f);
        if (progess <= 0f || progess >= 1f) {
            canPlatformMove = false;
        }
    }

    private void SlowTime(int playerNumber) {
        if (currentState == CurrentState.On) {
            SlowTimeAction?.Invoke(playerNumber);
            Debug.Log("something slowed down");
        }
        else {
            Debug.Log("something reset");
            ResetTimeAction?.Invoke(playerNumber);
        }
    }    
    
    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, checkPlayerRadius);
    }
}
