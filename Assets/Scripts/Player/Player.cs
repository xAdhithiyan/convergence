using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Update = Unity.VisualScripting.Update;
// ReSharper disable All

#if UNITY_EDITOR // => Ignore from here to next endif if not in editor
using UnityEditor;
using UnityEditorInternal;
#endif

public class Player : MonoBehaviour
{
    public int playerNumber;
    [Header("Movement")]
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float deceleration = 4f;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float currentSpeed;

    [Header("Jump")]
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private float jumpSpeed;

    [Header("Layers")]
    [SerializeField] private LayerMask jumpLayers;
    [SerializeField] private LayerMask spikeLayer;
    [SerializeField] private LayerMask slideLayer;
    [SerializeField] private LayerMask movingPlatformLayer;
    
    [Header("Scripts")]
    [SerializeField] public Lever[] lever;
    public MainPlayerInputs playerControls;
    
    private Rigidbody2D rb;
    private float currentdir = 0;
    private Vector2 movementInputs;
    private bool slideMovement = false;
    private float smoothTurn = 5; 
    private bool stayWithMovingPlatform = false;
    private GameObject currentPlatform;
    private Vector3 lastPlatformPosition;
    
    // value for slowing down when Time Slows
    private float timeSlowedBy = 1f;
    private float jumpReduction = 0f;
    private float defaultGravity = 2f;
    
    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        playerControls = new MainPlayerInputs();
        playerControls.Player.Move.performed += ctx => movementInputs = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => movementInputs = Vector2.zero; 
        playerControls.Player.Jump.performed += ctx => Jump();

        for (int i = 0; i < lever.Length; i++) {
            if (lever[i] != null) {
                int index = i; 
                playerControls.Player.Lever.performed += ctx => lever[index].ActivateLever(playerNumber);
            }
        }

        Lever.SlowTimeAction += HandleSlowTime;
        Lever.ResetTimeAction += HandleResetTime;
    }
    
    private void HandleResetTime(int num) {
        if (num != this.playerNumber && rb != null) {
            timeSlowedBy = 1f;
            jumpReduction = 0f;
            rb.gravityScale = defaultGravity;
        }
    }

    
    private void HandleSlowTime(int num) {
        if (num != this.playerNumber && rb != null) { 
            Debug.Log("hello");
            timeSlowedBy -= 0.5f;
            jumpReduction = 5.5f;
            rb.gravityScale = 0.6f;
        }
    }

    private void unSubscribe() {
        playerControls.Player.Move.performed -= ctx => movementInputs = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled -= ctx => movementInputs = Vector2.zero;
        playerControls.Player.Jump.performed -= ctx => Jump();

        for (int i = 0; i < lever.Length; i++) {
            if (lever[i] != null) {
                int index = i;
                playerControls.Player.Lever.performed -= ctx => lever[index].ActivateLever(playerNumber);
            }
        }
        
        
        Lever.SlowTimeAction -= HandleSlowTime;
        Lever.ResetTimeAction -= HandleResetTime;
        playerControls.Disable();
    }
    
    
    private void Start() { 
        currentSpeed = 0.1f;
    }
    
    private void OnEnable() { 
        playerControls.Enable();
    }
    
    private void OnDisable() {
        unSubscribe();
    }
    private void OnDestroy() {
        unSubscribe();
    }

    private void FixedUpdate() {
        Move(movementInputs, slideMovement);
        if (stayWithMovingPlatform) {
            Vector3 platformMovement = currentPlatform.transform.position - lastPlatformPosition;
            transform.position += platformMovement;
            lastPlatformPosition = currentPlatform.transform.position;
        }
    }

    private void Move(Vector2 FullDir, bool slideMovement = false) {
        float dir = FullDir.x;
        currentdir = Mathf.Lerp(currentdir, dir, Time.deltaTime * smoothTurn);
        
        if (slideMovement) {
            slideMove();
            return;
        }
        if (dir != 0) {
            if (currentSpeed < maxSpeed) {
                currentSpeed += acceleration * Time.deltaTime;
            }   
        } else {
            if (rb.velocity == Vector2.zero) {
                currentSpeed = 0;
            }
            else if(currentSpeed > 0) {
                currentSpeed -= deceleration * Time.deltaTime;
            }
        }

        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
        rb.velocity = new Vector2(currentdir * currentSpeed * timeSlowedBy, rb.velocity.y) ;
    }

    private void slideMove() {
        rb.velocity = new Vector2(rb.velocity.x * 1, rb.velocity.y);
    }

    private void Jump() {
        Collider2D isGrouded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, jumpLayers);
        if (isGrouded != null) {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed - jumpReduction);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (1 << collision.gameObject.layer == spikeLayer) {
            Death();
        }

        if (1 << collision.gameObject.layer == slideLayer) {
            slideMovement = true;
        }

        if (1 << collision.gameObject.layer == movingPlatformLayer) {
            stayWithMovingPlatform = true;
            currentPlatform = collision.gameObject; 
            lastPlatformPosition = currentPlatform.transform.position; 
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (1 << collision.gameObject.layer == slideLayer) {
            slideMovement = false;
        }
        if (1 << collision.gameObject.layer == movingPlatformLayer) {
            stayWithMovingPlatform = false;
        }
    }

    private void Death() {
        FindObjectOfType<AudioManager>().Play("death");
        Destroy(gameObject);
        GameManager.Instance.RestartScene();
    }

    public void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position, groundCheckRadius);
    }
}
