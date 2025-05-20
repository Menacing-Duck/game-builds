using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class SpellRuntime : NetworkBehaviour
{
    public SpellDefinitionBase def;
    public ulong casterId;
    public Team  casterTeam;

    Rigidbody2D rb;
    float spawnTime;

    public override void OnNetworkSpawn()
    {
        spawnTime = Time.time;
        rb = GetComponent<Rigidbody2D>();

        if (IsServer && def.lifetime > 0)
            Invoke(nameof(Despawn), def.lifetime);

        if (def is ProjectileSpellDefinition p && IsServer && rb != null)
            rb.linearVelocity = (Vector2)transform.up * p.projectileSpeed;
    }

    public void IgnoreCaster(Collider2D casterCol)
    {
        var myCol = GetComponent<Collider2D>();
        if (myCol && casterCol)
            Physics2D.IgnoreCollision(myCol, casterCol, true);
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        if (def is ProjectileSpellDefinition p)
        {
            if (!p.isHoming) return;
            if (Time.time - spawnTime < p.homingDelay) return;
            if (!TryFindTarget(out Vector2 dir)) return;

            Vector2 v = rb.linearVelocity;
            float angle = Vector2.SignedAngle(v, dir);
            float rot = Mathf.Clamp(angle, -p.homingStrength, p.homingStrength);
            rb.linearVelocity = Quaternion.Euler(0, 0, rot) * v;
        }
    }

    bool TryFindTarget(out Vector2 dir)
    {
        dir = Vector2.zero;
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 8);
        float best = float.MaxValue;
        foreach (var h in hits)
        {
            if (!h.TryGetComponent(out Stats s)) continue;
            if (!ShouldAffect(s)) continue;
            float d = Vector2.SqrMagnitude(h.transform.position - transform.position);
            if (d < best)
            {
                best = d;
                dir = (Vector2)(h.transform.position - transform.position);
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
        if (stat.IsAlly(casterTeam)) eff.damage = -Mathf.Abs(eff.damage);
        stat.Apply(eff, casterId);

        if (def is ProjectileSpellDefinition) Despawn();
    }

    void Despawn() => NetworkObject.Despawn();
}
