using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f;
    public float updateRate = 0.5f;
    public float stopDistance = 1.5f;
    public float targetUpdateInterval = 0.5f;
    public float pathUpdateInterval = 0.5f;
    public bool debugMode = false;

    private Pathfinding pathfinder;
    private int targetIndex;
    private List<Node> path;
    private Transform target;
    private Rigidbody2D rb;
    private bool isPathFinding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pathfinder = FindObjectOfType<Pathfinding>();
        
        if (pathfinder == null)
        {
            Debug.LogError("Pathfinding component not found in scene!");
            return;
        }

        StartCoroutine(UpdateTargetLoop());
        StartCoroutine(UpdatePathLoop());
    }

    IEnumerator UpdateTargetLoop()
    {
        while (true)
        {
            target = GetClosestPlayer();
            if (debugMode && target != null)
            {
                Debug.DrawLine(transform.position, target.position, Color.red, targetUpdateInterval);
            }
            yield return new WaitForSeconds(targetUpdateInterval);
        }
    }

    IEnumerator UpdatePathLoop()
    {
        while (true)
        {
            if (target != null && pathfinder != null)
            {
                float dist = Vector2.Distance(transform.position, target.position);
                if (dist > stopDistance && !isPathFinding)
                {
                    isPathFinding = true;
                    pathfinder.FindPath(transform.position, target.position);
                    yield return new WaitForSeconds(0.05f); 
                    path = pathfinder.currentPath;
                    targetIndex = 0;
                    isPathFinding = false;
                }
                else if (dist <= stopDistance)
                {
                    path = null;
                    AttackPlayer();
                }
            }

            yield return new WaitForSeconds(pathUpdateInterval);
        }
    }

    void FixedUpdate()
    {
        if (path == null || path.Count == 0 || targetIndex >= path.Count) return;

        Vector2 targetPos = path[targetIndex].worldPosition;
        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPos);

        if (debugMode)
        {
            for (int i = targetIndex; i < path.Count - 1; i++)
            {
                Debug.DrawLine(path[i].worldPosition, path[i + 1].worldPosition, Color.green, 0.1f);
            }
        }

        if (Vector2.Distance(rb.position, targetPos) < 0.1f)
        {
            targetIndex++;
        }
    }

    void AttackPlayer()
    {

        if (target != null)
        {
            Vector2 direction = ((Vector2)target.position - rb.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90); 
        }
    }

    Transform GetClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = player.transform;
            }
        }

        return closest;
    }
}