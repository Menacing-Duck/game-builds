using UnityEngine;
using Unity.Netcode;

public class EnemyAI : NetworkBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 3f;
    public float detectionRadius = 5f;
    public float stopDistance = 1.5f;
    public float obstacleCheckDistance = 0.5f;

    [Header("Default Destination")]
    public Transform defaultDestination;

    private Transform target;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private BoxCollider2D enemyCollider;
    private bool isMovingToDefaultDestination = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<BoxCollider2D>();

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    void Update()
    {
        if (!IsServer) return;

        FindNearestPlayer();

        if (target != null)
        {
            isMovingToDefaultDestination = false;
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget <= detectionRadius)
            {
                Vector2 direction = (target.position - transform.position).normalized;

                if (!IsPathBlocked(direction))
                {
                    if (distanceToTarget > stopDistance)
                    {
                        movement = direction;
                    }
                    else
                    {
                        movement = Vector2.zero;
                    }
                }
                else
                {
                    Vector2 alternativeDirection = FindAlternativePath(direction);
                    movement = alternativeDirection;
                }

                if (animator != null)
                {
                    animator.SetFloat("X", direction.x);
                    animator.SetFloat("Y", direction.y);
                    animator.SetFloat("Speed", movement.magnitude);
                    animator.SetBool("IsWalking", movement.magnitude > 0.1f);
                }
            }
            else
            {
                MoveToDefaultDestination();
            }
        }
        else if (defaultDestination != null)
        {
            MoveToDefaultDestination();
        }
    }

    void MoveToDefaultDestination()
    {
        if (defaultDestination == null) return;

        isMovingToDefaultDestination = true;
        Vector2 direction = (defaultDestination.position - transform.position).normalized;

        if (!IsPathBlocked(direction))
        {
            movement = direction;
        }
        else
        {
            Vector2 alternativeDirection = FindAlternativePath(direction);
            movement = alternativeDirection;
        }

        if (animator != null)
        {
            animator.SetFloat("X", direction.x);
            animator.SetFloat("Y", direction.y);
            animator.SetFloat("Speed", movement.magnitude);
            animator.SetBool("IsWalking", movement.magnitude > 0.1f);
        }
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        
        if (rb != null)
        {
            rb.linearVelocity = movement * moveSpeed;
        }
    }

    bool IsPathBlocked(Vector2 direction)
    {
        if (enemyCollider == null) return false;

        RaycastHit2D[] hits = Physics2D.CircleCastAll(
            transform.position,
            enemyCollider.bounds.extents.x,
            direction,
            obstacleCheckDistance
        );

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;
            if (target != null && hit.collider.gameObject == target.gameObject) continue;
            return true;
        }

        return false;
    }

    Vector2 FindAlternativePath(Vector2 originalDirection)
    {
        float[] angles = { 45f, -45f, 90f, -90f, 135f, -135f };
        foreach (float angle in angles)
        {
            Vector2 rotatedDirection = Quaternion.Euler(0, 0, angle) * originalDirection;
            if (!IsPathBlocked(rotatedDirection))
            {
                return rotatedDirection;
            }
        }

        Vector2 perpendicular = new Vector2(-originalDirection.y, originalDirection.x);
        if (!IsPathBlocked(perpendicular))
        {
            return perpendicular;
        }

        return Random.insideUnitCircle.normalized;
    }

    void FindNearestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = float.MaxValue;
        Transform closestPlayer = null;

        foreach (GameObject player in players)
        {
            Stats playerStats = player.GetComponent<Stats>();
            if (playerStats != null && playerStats.health.Value <= 0) continue;

            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player.transform;
            }
        }

        target = closestPlayer;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        if (enemyCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, enemyCollider.bounds.extents.x);
        }

        if (defaultDestination != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, defaultDestination.position);
        }
    }
} 