using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class SpellCaster : NetworkBehaviour
{
    public List<SpellDefinitionBase> spellbook = new(5);

    float[] lastCast = new float[5];
    Stats   stats;

    static readonly string[] defaults = { "F", "Mouse0", "Q", "E", "V" };

    void Awake() => stats = GetComponent<Stats>();

    void Update()
    {
        if (!IsOwner) return;

        for (int slot = 0; slot < spellbook.Count; slot++)
            HandleSlot(slot);
    }



    KeyCode BoundKey(int slot)
    {
        string keyStr = PlayerPrefs.GetString($"spell{slot}", defaults[slot]);
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), keyStr);
    }

    void HandleSlot(int slot)
    {
        if (slot >= spellbook.Count) return;
        var spell = spellbook[slot];
        if (!spell) return;

        KeyCode key = BoundKey(slot);

        bool trigger;
        if (spell is ProjectileSpellDefinition p && p.automaticFire)
            trigger = Input.GetKey(key);
        else
            trigger = Input.GetKeyDown(key);

        if (trigger) TryFire(slot);
    }



    void TryFire(int slot)
    {
        var spell = spellbook[slot];
        if (Time.time - lastCast[slot] < spell.cooldown) return;
        if (stats.mana.Value < spell.manaCost)          return;

        Vector2 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CastServerRpc(slot, aimPos);
        lastCast[slot] = Time.time;
    }

    [ServerRpc]
    void CastServerRpc(int slot, Vector2 aim)
    {
        if (slot >= spellbook.Count) return;
        var spell = spellbook[slot];
        if (stats.mana.Value < spell.manaCost) return;
        stats.mana.Value -= spell.manaCost;

        if (spell is ProjectileSpellDefinition p)
        {
            GameObject proj = Instantiate(
                p.projectilePrefab,
                transform.position,
                Quaternion.LookRotation(Vector3.forward, aim - (Vector2)transform.position));

            var runtime = proj.GetComponent<SpellRuntime>();
            runtime.def        = spell;
            runtime.casterId   = OwnerClientId;
            runtime.casterTeam = stats.team;

            Collider2D casterCol = GetComponent<Collider2D>();
            runtime.IgnoreCaster(casterCol);

            proj.GetComponent<NetworkObject>().Spawn(true);
        }
        else if (spell is AoeSpellDefinition a)
        {
            Vector2 casterPos = transform.position;
            Vector2 dir = (aim - casterPos).normalized;
            if (dir.sqrMagnitude == 0) dir = Vector2.up;

            Vector3 spawnPos;
            Quaternion rot;

            if (a.spawnMode == AoeSpawnMode.AtMouse)
            {
                spawnPos = aim;
                rot = Quaternion.identity;
            }
            else
            {
                spawnPos = casterPos + dir * a.offsetFromCaster;
                rot = Quaternion.LookRotation(Vector3.forward, dir);
            }

            GameObject aoeObj = Instantiate(a.aoePrefab, spawnPos, rot);

            var runtime = aoeObj.GetComponent<SpellRuntime>();
            runtime.def        = spell;
            runtime.casterId   = OwnerClientId;
            runtime.casterTeam = stats.team;

            aoeObj.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
