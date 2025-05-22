using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class SpellCaster : NetworkBehaviour
{
    public List<SpellDefinitionBase> spellbook = new(5);

    float[] lastCast = new float[5];
    Stats stats;
    static readonly string[] defaults = { "F", "Mouse0", "Q", "E", "V" };

    void Awake() => stats = GetComponent<Stats>();

    void Update()
    {
        if (!IsOwner) return;
        for (int slot = 0; slot < spellbook.Count; slot++)
            HandleSlot(slot);
    }

    void HandleSlot(int slot)
    {
        if (slot >= spellbook.Count) return;
        var spell = spellbook[slot];
        if (spell == null) return;
        KeyCode key = BoundKey(slot);
        bool trigger = (spell is ProjectileSpellDefinition p && p.automaticFire)
            ? Input.GetKey(key)
            : Input.GetKeyDown(key);
        if (trigger) TryFire(slot);
    }

    KeyCode BoundKey(int slot)
    {
        string k = PlayerPrefs.GetString($"spell{slot}", defaults[slot]);
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), k);
    }



void TryFire(int slot)
{
    var spell = spellbook[slot];
    float cd = spell.cooldown * stats.GetCooldownMultiplier();
    if (Time.time - lastCast[slot] < cd) return;

    int cost = Mathf.Max(0, Mathf.RoundToInt(spell.manaCost * stats.GetManaCostMultiplier()));
    if (stats.mana.Value < cost) return;

    Vector2 aimPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    CastServerRpc(slot, aimPos, cost);
    lastCast[slot] = Time.time;
}



    [ServerRpc]
    void CastServerRpc(int slot, Vector2 aim,int cost)
    {
        if (slot >= spellbook.Count) return;
        var spell = spellbook[slot];
        if (stats.mana.Value < cost) return;
        stats.mana.Value -= cost;

        if (spell is ProjectileSpellDefinition p)
        {
            GameObject proj = Instantiate(
                p.projectilePrefab,
                transform.position,
                Quaternion.LookRotation(Vector3.forward, aim - (Vector2)transform.position));
            var runtime = proj.GetComponent<SpellRuntime>();
            runtime.def = spell;
            runtime.casterId = OwnerClientId;
            runtime.casterTeam = stats.team;
            Collider2D col = GetComponent<Collider2D>();
            runtime.IgnoreCaster(col);
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

    var netObj = aoeObj.GetComponent<NetworkObject>();
    netObj.Spawn(true);

    if (a.stickToCaster)
        aoeObj.transform.SetParent(transform, true);
}

        else if (spell is DashSpellDefinition d)
        {
            Vector2 dir = (aim - (Vector2)transform.position).normalized;
            if (dir.sqrMagnitude == 0) dir = Vector2.up;
            Vector2 target = (Vector2)transform.position + dir * d.dashDistance;
            var rpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { OwnerClientId } }
            };
            PerformDashClientRpc(target, d.ignoreCollisions, d.dashTime, rpcParams);
        }
    }

    [ClientRpc]
    void PerformDashClientRpc(Vector2 target, bool ignoreCollisions, float dashTime, ClientRpcParams rpcParams = default)
    {
        Collider2D col = GetComponent<Collider2D>();
        if (ignoreCollisions && col != null)
        {
            col.enabled = false;
            StartCoroutine(ReEnable(col, dashTime));
        }
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb) rb.MovePosition(target);
        else transform.position = target;
    }

    IEnumerator ReEnable(Collider2D c, float t)
    {
        yield return new WaitForSeconds(t);
        if (c) c.enabled = true;
    }
}
