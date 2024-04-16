using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {
    [SerializeField] private float moveSpeedMultiplier = 1;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask goalLayer;

    [SerializeField] private GameObject blockPrefab;

    private SpriteRenderer sr;
    private BlockPool blockPool;

    private bool moving;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
        blockPool = FindObjectOfType<BlockPool>();
    }

    void Update() {
        if (moving) {
            return;
        }

        if (Input.GetButtonDown("Restart")) {
            // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            EventBus.instance.TriggerOnLevelRestart();
        }

        if (Input.GetButtonDown("Block")) {
            Vector3 checkPos = transform.position + (!sr.flipX ? Vector3.left : Vector3.right);

            if (!Physics2D.OverlapCircle(checkPos, 0.1f, groundLayer)) {
                // Nothing here, can overlap
                blockPool.EnableBlockAtPos(checkPos);
            } else {
                // Destroy(blockPool.GetBlockAtPos(checkPos)?.gameObject);
                blockPool.DisableBlockAtPos(checkPos);
            }
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 end;

        if (horizontal > 0) {
            end = transform.position + Vector3.right;
        } else if (horizontal < 0) {
            end = transform.position + Vector3.left;
        } else {
            return;
        }
        Vector3 p1 = transform.position;
        Vector3 p2 = end;

        if (Physics2D.OverlapCircle(end, 0.1f, groundLayer)) {
            // Check above, if free let's move there
            Vector3 currUpCheck = transform.position + Vector3.up;
            Vector3 upCheck = end + Vector3.up;
            if (!Physics2D.OverlapCircle(currUpCheck, 0.1f, groundLayer) && !Physics2D.OverlapCircle(upCheck, 0.1f, groundLayer)) {
                // Move here, set p1, p2 and end points
                p1 = currUpCheck;
                p2 = p1;
                end = upCheck;

                StartCoroutine(Move(p1, p2, end));
            }
        } else {
            // Space is free, but check below
            Vector3 downCheck = end + Vector3.down;

            if (Physics2D.OverlapCircle(downCheck, 0.1f, groundLayer)) {
                StartCoroutine(Move(p1, p2, end));
            } else {
                // If ground is missing, we need to fall
                p1 = end;
                p2 = end;
                end = downCheck;

                StartCoroutine(Move(p1, p2, end));
            }
        }
    }

    IEnumerator Move(Vector3 p1, Vector3 p2, Vector3 end) {
        moving = true;

        Vector3 start = transform.position;
        float t = 0;

        sr.flipX = start.x < end.x;

        while (t < 1) {
            transform.position = Bezier(start, p1, p2, end, t);
            yield return null;
            t += Time.deltaTime * moveSpeedMultiplier;
        }
        transform.position = end;

        // Keep moving down if there's space
        while (!Physics2D.OverlapCircle(transform.position + Vector3.down, 0.1f, groundLayer)) {
            Vector3 fallStart = transform.position;
            Vector3 fallEnd = fallStart + Vector3.down;
            t = 0;

            while (t < 1) {
                transform.position = Vector3.Lerp(fallStart, fallEnd, t);
                yield return null;
                t += Time.deltaTime * moveSpeedMultiplier;
            }
            transform.position = fallEnd;
        }

        // Check goal
        if (Physics2D.OverlapCircle(transform.position, 0.1f, goalLayer)) {
            // int currBuildIndex = SceneManager.GetActiveScene().buildIndex;
            // SceneManager.LoadScene((currBuildIndex + 1) % SceneManager.sceneCountInBuildSettings);
            EventBus.instance.TriggerOnLevelComplete();
        } else {
            moving = false;
        }
    }
    
    public Vector3 Bezier(Vector3 start, Vector3 p1, Vector3 p2, Vector3 end, float t) {
        float tInv = 1 - t;
        return tInv * tInv * tInv * start +
               3 * tInv * tInv * t * p1 +
               3 * tInv * t * t * p2 +
               t * t * t * end;
        // return (((-start + 3*(p1-p2) + end)* t + (3*(start+p2) - 6*p1))* t + 3*(p1-start))* t + start;
    }
}