using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkObject))]
public class Stats : NetworkBehaviour
{
    public int maxHealth = 100;
    public int maxMana   = 100;
    public float healthRegen = 2;
    public float manaRegen   = 4;
    public NetworkVariable<int> health = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> mana   = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public Team team = Team.Neutral;

    struct Buff
    {
        public float reduction;
        public float manaBonus;
        public float costReduction;
        public float remaining;
    }

    readonly List<Buff> buffs = new();

    void Update()
    {
        if (!IsServer) return;
        float dt = Time.deltaTime;
        float effManaRegen = manaRegen;
        float totalCostRed = 0f;
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            var b = buffs[i];
            b.remaining -= dt;
            if (b.remaining <= 0f) buffs.RemoveAt(i);
            else
            {
                effManaRegen += b.manaBonus;
                totalCostRed += b.costReduction;
                buffs[i] = b;
            }
        }
        if (health.Value < maxHealth)
            health.Value = Mathf.Min(maxHealth, health.Value + Mathf.RoundToInt(healthRegen * dt));
        if (mana.Value < maxMana)
            mana.Value   = Mathf.Min(maxMana,   mana.Value   + Mathf.RoundToInt(effManaRegen * dt));
    }

    public void Apply(SpellEffect eff, ulong casterId)
    {
        if (eff.cooldownReduction != 0f || eff.manaRegenBonus != 0f || eff.manaCostReduction != 0f)
            buffs.Add(new Buff { reduction = eff.cooldownReduction, manaBonus = eff.manaRegenBonus, costReduction = eff.manaCostReduction, remaining = eff.duration });
        if (eff.duration <= 0f || eff.tickInterval <= 0f)
            health.Value = Mathf.Clamp(health.Value - eff.damage, 0, maxHealth);
        else
            StartCoroutine(HandlePeriodic(eff));
    }

    System.Collections.IEnumerator HandlePeriodic(SpellEffect eff)
    {
        float rem = eff.duration;
        while (rem > 0f)
        {
            health.Value = Mathf.Clamp(health.Value - eff.damage, 0, maxHealth);
            yield return new WaitForSeconds(eff.tickInterval);
            rem -= eff.tickInterval;
        }
    }

    public bool IsAlly(Team other) => team != Team.Neutral && team == other;

    public float GetCooldownMultiplier()
    {
        float total = 0f;
        foreach (var b in buffs) total += b.reduction;
        return Mathf.Clamp01(1f - total);
    }

    public float GetManaCostMultiplier()
    {
        float total = 0f;
        foreach (var b in buffs) total += b.costReduction;
        return Mathf.Clamp01(1f - total);
    }
}
