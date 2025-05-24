using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.UI;

public class DeathManager : NetworkBehaviour
{
    public float respawnDelay = 3f;
    public float invincibilityDuration = 2f;
    public Image deathOverlay;
    public float fadeSpeed = 1f;
    private Stats stats;
    private bool isDead = false;
    public Transform respawnPoint;


    void Start()
    {
        stats = GetComponent<Stats>();
        if (deathOverlay != null)
        {
            Color color = deathOverlay.color;
            color.a = 0f;
            deathOverlay.color = color;
        }
    }

    void Update()
    {
        if (stats != null)
        {
            if (stats.health.Value <= 0 && !isDead)
            {
                HandleDeath();
            }
        }
    }

    void HandleDeath()
    {
        isDead = true;
        
        var movement = GetComponent<PlayerMovement>();
        if (movement != null) 
        {
            movement.enabled = false;
        }
        
        var spellCaster = GetComponent<SpellCaster>();
        if (spellCaster != null) 
        {
            spellCaster.enabled = false;
        }

        StartCoroutine(RespawnAfterDelay());
        StartCoroutine(FadeDeathOverlay(true));
    }

    IEnumerator FadeDeathOverlay(bool fadeIn)
    {
        if (deathOverlay == null) yield break;

        float targetAlpha = fadeIn ? 0.7f : 0f;
        Color color = deathOverlay.color;
        float startAlpha = color.a;

        float elapsedTime = 0f;
        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeSpeed);
            deathOverlay.color = color;
            yield return null;
        }

        color.a = targetAlpha;
        deathOverlay.color = color;
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            transform.position = Vector3.zero;
        }

        if (stats != null)
        {
            stats.health.Value = stats.maxHealth;
            stats.Mana.Value = 10;
        }
        
        isDead = false;

        var movement = GetComponent<PlayerMovement>();
        if (movement != null) 
        {
            movement.enabled = true;
        }
        
        var spellCaster = GetComponent<SpellCaster>();
        if (spellCaster != null) 
        {
            spellCaster.enabled = true;
        }

        StartCoroutine(FadeDeathOverlay(false));
        StartCoroutine(ApplyInvincibility());
    }

    IEnumerator ApplyInvincibility()
    {
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        }

        yield return new WaitForSeconds(invincibilityDuration);

        if (collider != null)
        {
            collider.enabled = true;
        }

        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }
} 