using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;

public class SpellCaster : NetworkBehaviour
{
    public List<SpellDefinitionBase> spellbook = new(5);

    float[] lastCast;
    float[] chargeStart;
    public int[] currentAmmo;
    public int[] ammoPool;
    public bool[] reloading;
    Stats stats;
    static readonly string[] defaults = { "F", "Mouse0", "Q", "E", "V" };

    void Awake()
    {
        stats = GetComponent<Stats>();
        int n = spellbook.Count;
        lastCast     = new float[n];
        chargeStart  = new float[n];
        currentAmmo  = new int[n];
        ammoPool     = new int[n];
        reloading    = new bool[n];
        for (int i = 0; i < n; i++)
        {
            var s = spellbook[i];
            if (s != null && s.magazineSize > 0)
            {
                currentAmmo[i] = s.magazineSize;
                ammoPool[i]    = s.totalAmmo > 0 ? s.totalAmmo - s.magazineSize : int.MaxValue;
            }
            else
            {
                currentAmmo[i] = ammoPool[i] = int.MaxValue;
            }
        }
    }

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

        if (spell is ProjectileSpellDefinition p && p.isChargeable)
        {
            if (Input.GetKeyDown(key))
                chargeStart[slot] = Time.time;
            if (Input.GetKeyUp(key))
            {
                float ratio = Mathf.Clamp01((Time.time - chargeStart[slot]) / p.maxChargeTime);
                TryFireCharged(slot, ratio);
            }
        }
        else
        {
            bool trigger;
            if (spell is ProjectileSpellDefinition pp && pp.automaticFire)
                trigger = Input.GetKey(key);
            else if (spell is AoeSpellDefinition aa && aa.automaticFire)
                trigger = Input.GetKey(key);
            else
                trigger = Input.GetKeyDown(key);

            if (trigger && !reloading[slot])
                TryFire(slot);
        }
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
        if (spell.magazineSize > 0 && currentAmmo[slot] <= 0)
        {
            StartCoroutine(Reload(slot, spell.reloadTime));
            return;
        }
        int cost = spell.magazineSize > 0
            ? 0
            : Mathf.Max(0, Mathf.RoundToInt(spell.manaCost * stats.GetManaCostMultiplier()));
        if (stats.Mana.Value < cost) return;
        Vector2 aim = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CastServerRpc(slot, aim, cost, 1f);
        lastCast[slot] = Time.time;
        if (spell.magazineSize > 0)
        {
            currentAmmo[slot]--;
            if (ammoPool[slot] != int.MaxValue)
                ammoPool[slot]--;
        }
    }

    void TryFireCharged(int slot, float chargeRatio)
    {
        var spell = spellbook[slot];
        float cd = spell.cooldown * stats.GetCooldownMultiplier();
        if (Time.time - lastCast[slot] < cd) return;
        if (spell.magazineSize > 0 && currentAmmo[slot] <= 0)
        {
            StartCoroutine(Reload(slot, spell.reloadTime));
            return;
        }
        int cost = spell.magazineSize > 0
            ? 0
            : Mathf.Max(0, Mathf.RoundToInt(spell.manaCost * stats.GetManaCostMultiplier()));
        if (stats.Mana.Value < cost) return;
        Vector2 aim = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CastServerRpc(slot, aim, cost, chargeRatio);
        lastCast[slot] = Time.time;
        if (spell.magazineSize > 0)
        {
            currentAmmo[slot]--;
            if (ammoPool[slot] != int.MaxValue)
                ammoPool[slot]--;
        }
    }

    IEnumerator Reload(int slot, float reloadTime)
    {
        reloading[slot] = true;
        yield return new WaitForSeconds(reloadTime);
        var s = spellbook[slot];
        int load = s.magazineSize;
        if (ammoPool[slot] != int.MaxValue)
            load = Mathf.Min(load, ammoPool[slot]);
        currentAmmo[slot] = load;
        reloading[slot]   = false;
    }

    [ServerRpc]
    void CastServerRpc(int slot, Vector2 aim, int cost, float charge)
    {
        if (slot >= spellbook.Count) return;
        var spell = spellbook[slot];
        if (spell.magazineSize == 0)
        {
            if (stats.Mana.Value < cost) return;
            stats.Mana.Value -= cost;
        }



    if (spell is ProjectileSpellDefinition p)
    {
        float speed = p.projectileSpeed;
        if (p.isChargeable)
            speed *= Mathf.Lerp(p.minSpeedPercent, p.maxSpeedPercent, charge);

        int baseDmg = p.effect.damage;
        int dmg = p.isChargeable
            ? Mathf.RoundToInt(baseDmg * Mathf.Lerp(p.minDamagePercent, p.maxDamagePercent, charge))
            : baseDmg;

        GameObject proj = Instantiate(
            p.projectilePrefab,
            transform.position,
            Quaternion.LookRotation(Vector3.forward, aim - (Vector2)transform.position));

        var runtime = proj.GetComponent<SpellRuntime>();
        runtime.def           = spell;
        runtime.overrideSpeed = speed;
        runtime.overrideDamage= dmg;
        runtime.casterId      = OwnerClientId;
        runtime.casterTeam    = stats.team;

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
        else if (spell is AttachSpellDefinition at)
        {
            Collider2D hit = Physics2D.OverlapPoint(aim);
            if (hit)
            {
                GameObject go = hit.gameObject;
                Vector3 pos = go.transform.position + (Vector3)(Vector2.up * at.offsetFromCenter);
                GameObject att = Instantiate(at.attachPrefab, pos, Quaternion.identity);
                att.transform.SetParent(go.transform, true);
                var runtime = att.GetComponent<SpellRuntime>();
                runtime.def        = spell;
                runtime.casterId   = OwnerClientId;
                runtime.casterTeam = stats.team;
                att.GetComponent<NetworkObject>().Spawn(true);
            }
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
