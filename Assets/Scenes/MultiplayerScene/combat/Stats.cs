using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(NetworkObject))]
public class Stats : NetworkBehaviour
{
    public int maxHealth = 100;
    public int maxMana   = 100;
    public float healthRegen = 2;
    public float manaRegen   = 4;

    public NetworkVariable<int> health = new(100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public NetworkVariable<int> mana = new(100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public Team team = Team.Neutral;

    void Update()
    {
        if (!IsServer) return;
        if (health.Value < maxHealth)
            health.Value = Mathf.Min(maxHealth, health.Value + Mathf.RoundToInt(healthRegen * Time.deltaTime));
        if (mana.Value   < maxMana)
            mana.Value   = Mathf.Min(maxMana,   mana.Value   + Mathf.RoundToInt(manaRegen   * Time.deltaTime));
    }

    public void Apply(SpellEffect eff, ulong casterId)
    {
        health.Value = Mathf.Clamp(health.Value - eff.damage, 0, maxHealth);
        if (health.Value == 0)
        {
            Debug.Log($"{name} killed by client {casterId}");
        }
    }

    public bool IsAlly(Team other) => team != Team.Neutral && team == other;
}
