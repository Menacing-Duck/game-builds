using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.UI;

public class DeathManager : NetworkBehaviour
{
    public float respawnDelay = 3f;
    public float invincibilityDuration = 2f;
    public Transform respawnPoint; // Point de respawn fixe
    public Image deathOverlay; // Image pour l'effet de gris
    public float fadeSpeed = 1f; // Vitesse de fondu
    private Stats stats;
    private bool isDead = false;

    void Start()
    {
        stats = GetComponent<Stats>();
        if (stats == null)
        {
            Debug.LogError("Stats component not found!");
        }

        // S'assurer que l'overlay est invisible au démarrage
        if (deathOverlay != null)
        {
            Color color = deathOverlay.color;
            color.a = 0f;
            deathOverlay.color = color;
        }
    }

    void Update()
    {
        if (!IsServer) return;

        // Vérifier si le joueur est mort
        if (stats.health.Value <= 0 && !isDead)
        {
            HandleDeath();
        }
    }

    void HandleDeath()
    {
        isDead = true;
        
        // Désactiver les composants du joueur
        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;
        
        var spellCaster = GetComponent<SpellCaster>();
        if (spellCaster != null) spellCaster.enabled = false;

        // Démarrer les effets de mort
        StartCoroutine(RespawnAfterDelay());
        StartCoroutine(FadeDeathOverlay(true));
    }

    IEnumerator FadeDeathOverlay(bool fadeIn)
    {
        if (deathOverlay == null) yield break;

        float targetAlpha = fadeIn ? 0.7f : 0f; // 0.7 pour un gris semi-transparent
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

        if (!IsServer) yield break;

        // Téléporter le joueur au point de respawn
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
        }
        else
        {
            transform.position = Vector3.zero;
        }

        // Réinitialiser les stats
        stats.health.Value = stats.maxHealth;
        stats.mana.Value = stats.maxMana;
        isDead = false;

        // Réactiver les composants
        var movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = true;
        
        var spellCaster = GetComponent<SpellCaster>();
        if (spellCaster != null) spellCaster.enabled = true;

        // Faire disparaître l'overlay
        StartCoroutine(FadeDeathOverlay(false));

        // Appliquer l'invincibilité temporaire
        StartCoroutine(ApplyInvincibility());
    }

    IEnumerator ApplyInvincibility()
    {
        // Désactiver temporairement les collisions
        var collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Effet visuel d'invincibilité
        var spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        }

        yield return new WaitForSeconds(invincibilityDuration);

        // Réactiver les collisions
        if (collider != null)
        {
            collider.enabled = true;
        }

        // Restaurer l'apparence normale
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