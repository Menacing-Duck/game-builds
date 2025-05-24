using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkObject))]
public class Stats : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> CurLvl = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> MaxMana = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);  
    public float healthRegen = 2;
    public float manaRegen   = 4;

    public int Money = 10;

    public NetworkVariable<int> health = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<int> Mana   = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public Team team = Team.Neutral;

    private DeathManager deathManager;

    struct Buff
    {
        public float reduction;
        public float manaBonus;
        public float costReduction;
        public float remaining;
    }

    readonly List<Buff> buffs = new();

    float healthRegenAcc;
    float manaRegenAcc;

    void Start()
    {
        deathManager = GetComponent<DeathManager>();
        if (deathManager == null)
        {
            Debug.LogWarning("DeathManager component not found!");
        }
    }

    void Update()
    {
        if (!IsServer) return;
        float dt = Time.deltaTime;

        if (deathManager != null && deathManager.IsDead()) return;

        float totalManaBonus = 0f;
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            var b = buffs[i];
            b.remaining -= dt;
            if (b.remaining <= 0f) buffs.RemoveAt(i);
            else
            {
                totalManaBonus += b.manaBonus;
                buffs[i] = b;
            }
        }

        healthRegenAcc += healthRegen * dt;
        int hGain = (int)healthRegenAcc;
        if (hGain > 0)
        {
            health.Value = Mathf.Min(maxHealth, health.Value + hGain);
            healthRegenAcc -= hGain;
        }

        manaRegenAcc += (manaRegen + totalManaBonus) * dt;
        int mGain = (int)manaRegenAcc;
        if (mGain > 0)
        {
            Mana.Value = Mathf.Min(MaxMana.Value, Mana.Value + mGain);
            manaRegenAcc -= mGain;
        }
    }

    public void Apply(SpellEffect eff, ulong casterId)
    {
        if (deathManager != null && deathManager.IsDead()) return;

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
        while (rem > 0f && (deathManager == null || !deathManager.IsDead()))
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
