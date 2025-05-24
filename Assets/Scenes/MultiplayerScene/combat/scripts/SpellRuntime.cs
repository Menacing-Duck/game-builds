// SpellRuntime.cs
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class SpellRuntime : NetworkBehaviour
{
    public SpellDefinitionBase def;
    public float overrideSpeed = -1f;
    public int   overrideDamage = int.MinValue;
    public ulong casterId;
    public Team  casterTeam;

    Rigidbody2D rb;
    float spawnTime;
    int remainingPierces;

    public override void OnNetworkSpawn()
    {
        spawnTime = Time.time;
        rb = GetComponent<Rigidbody2D>();

        if (def is ProjectileSpellDefinition p)
            remainingPierces = p.pierceCount;

        if (IsServer && def.lifetime > 0f)
            Invoke(nameof(Despawn), def.lifetime);

        if (def is ProjectileSpellDefinition p2 && IsServer && rb != null)
        {
            float speed = overrideSpeed > 0f ? overrideSpeed : p2.projectileSpeed;
            rb.linearVelocity = (Vector2)transform.up * speed;
        }
    }

    public void IgnoreCaster(Collider2D casterCol)
    {
        Collider2D myCol = GetComponent<Collider2D>();
        if (myCol != null && casterCol != null)
            Physics2D.IgnoreCollision(myCol, casterCol, true);
    }

    void Update()
    {
        if (def is ProjectileSpellDefinition && rb != null)
        {
            Vector2 v = rb.linearVelocity;
            if (v.sqrMagnitude > 0.001f)
                transform.up = v.normalized;
        }
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        if (def is ProjectileSpellDefinition p && p.isHoming)
        {
            if (Time.time - spawnTime < p.homingDelay) return;
            if (!TryFindTarget(out Vector2 dir)) return;
            Vector2 v = rb.linearVelocity;
            float ang = Vector2.SignedAngle(v, dir);
            float rot = Mathf.Clamp(ang, -p.homingStrength, p.homingStrength);
            rb.linearVelocity = Quaternion.Euler(0, 0, rot) * v;
        }
    }

    bool TryFindTarget(out Vector2 dir)
    {
        dir = Vector2.zero;
        float best = float.MaxValue;
        foreach (var col in Physics2D.OverlapCircleAll(transform.position, 8f))
        {
            if (!col.TryGetComponent(out Stats s)) continue;
            if (!ShouldAffect(s)) continue;
            float d = (col.transform.position - transform.position).sqrMagnitude;
            if (d < best)
            {
                best = d;
                dir = (Vector2)(col.transform.position - transform.position);
            }
        }
        return best < float.MaxValue;
    }

    bool ShouldAffect(Stats target)
    {
        if (target.team == Team.Neutral) return false;
        if (target.IsAlly(casterTeam))   return def.affectAllies;
        return def.affectEnemies;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!IsServer) return;
        if (!col.TryGetComponent(out Stats stat)) return;
        if (!ShouldAffect(stat)) return;

        SpellEffect eff = def.effect;
        if (overrideDamage != int.MinValue)
            eff.damage = overrideDamage;
        if (stat.IsAlly(casterTeam))
            eff.damage = -Mathf.Abs(eff.damage);

        stat.Apply(eff, casterId);

        if (def is ProjectileSpellDefinition p)
        {
            if (remainingPierces > 0)
            {
                remainingPierces--;
                return;
            }
        }

        Despawn();
    }

    void Despawn() => NetworkObject.Despawn();
}
