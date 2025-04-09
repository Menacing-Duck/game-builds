using System.Collections;
using UnityEngine;

public class DashAbility : Ability
{
    public float dashSpeed = 100000f;
    public float dashDuration = 1f;
    private Rigidbody2D rb;
    private bool isDashing = false;
    private Vector2 lastMoveDirection = Vector2.right; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        AbilityName = "Dash"; 
        cooldown = 0.2f;
    }

    public override void Activate()
    {
        if (!isOnCooldown && !isDashing)
        {
            StartCoroutine(DashRoutine());
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;

        Vector2 dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (dashDirection != Vector2.zero)
            lastMoveDirection = dashDirection;
        rb.linearVelocity = lastMoveDirection * dashSpeed;
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        yield return new WaitForSeconds(dashDuration);

        if (movement != null)
            movement.enabled = true;

        rb.linearVelocity = Vector2.zero;
        isDashing = false;
    }
}
