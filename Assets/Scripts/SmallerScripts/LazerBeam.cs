using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerBeam : MonoBehaviour {
    private Vector2[] direction = new Vector2[] {
        Vector2.up, Vector2.right, Vector2.down, Vector2.left,
        new Vector2(1,1).normalized, new Vector2(1,-1).normalized, new Vector2(-1,1).normalized, new Vector2(-1,-1).normalized 
    };

    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private float rotataionSpeed;
    [SerializeField] private LineRenderer[] lineRenderers = new LineRenderer[8];
    private float maxDistance = 100f;

    private void Start() {
        for (int i = 0; i < lineRenderers.Length; i++) {
            GameObject lineObject = new GameObject($"LineRenderer_{i}");
            lineObject.transform.parent = transform; 
            lineRenderers[i] = lineObject.AddComponent<LineRenderer>();
            lineRenderers[i].sortingOrder = 3;
            
            lineRenderers[i].startWidth = 0.1f;
            lineRenderers[i].endWidth = 0.1f;
            lineRenderers[i].material = new Material(Shader.Find("Sprites/Default"));
            lineRenderers[i].startColor = Color.red;
            lineRenderers[i].endColor = Color.red;
            lineRenderers[i].positionCount = 2;
        }
    }


    private void Update() {
        CastRay();
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + Time.deltaTime * rotataionSpeed);
    }

    private void CastRay() {
        for (int i = 0; i < lineRenderers.Length; i++) {
            Vector2 worldSpaceVec = transform.TransformDirection(direction[i]).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, worldSpaceVec, maxDistance, collisionMask);

            if (hit.collider != null) {
                if (hit.collider.gameObject.CompareTag("Player")) {
                    GameManager.Instance.RestartScene();
                }
                
                lineRenderers[i].SetPosition(0, transform.position);
                lineRenderers[i].SetPosition(1, hit.point);
            }
            else {
                lineRenderers[i].SetPosition(0, transform.position);
                lineRenderers[i].SetPosition(1, (Vector2)transform.position + worldSpaceVec * maxDistance);
            }
        }

    }
}
