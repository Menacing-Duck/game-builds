using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEditor;

public class NetPlayer : NetworkBehaviour
{
    public int Uid;
    public TMP_Text DebugInfo;
    public SpellCaster caster;
    public Stats stats;

    void Start()
    {
        stats = GetComponent<Stats>();
        caster = GetComponent<SpellCaster>();
    }

    void Update()
    {
        if (!IsOwner) return;
        // ShowDebugInfo();
    }

    void ShowDebugInfo()
{
    if (DebugInfo == null || stats == null || caster == null) return;

    var sb = new System.Text.StringBuilder();

    sb.AppendLine($"UID {Uid}  Client {OwnerClientId}");
    sb.AppendLine($"Team {stats.team}");
    sb.AppendLine($"HP {stats.health.Value}/{stats.maxHealth}  (+{stats.healthRegen}/s)");
    sb.AppendLine($"MP {stats.Mana.Value}/{stats.MaxMana}  (+{stats.manaRegen}/s)");

    Vector2 pos = transform.position;
    sb.AppendLine($"Pos {pos.x:0.00},{pos.y:0.00}");

    if (TryGetComponent<Rigidbody2D>(out var rb))
        sb.AppendLine($"Vel {rb.linearVelocity.x:0.00},{rb.linearVelocity.y:0.00}");

    for (int i = 0; i < caster.spellbook.Count; i++)
    {
        var s = caster.spellbook[i];
        if (!s) continue;
        sb.AppendLine($"{i} {s.spellName}  CD {s.cooldown}s  Mana {s.manaCost}");
    }

    DebugInfo.text = sb.ToString();
}

    void HideDebugInfo()
    {
        DebugInfo.text = "";
    }

}
