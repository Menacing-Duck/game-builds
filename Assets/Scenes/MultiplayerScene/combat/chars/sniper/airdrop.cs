using UnityEngine;

public class AmmoManaRechargeZone : MonoBehaviour
{
    public int manaRestoreAmount = 10;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var stats = other.GetComponent<Stats>();
        var caster = other.GetComponent<SpellCaster>();
        if (stats != null)
        {
            stats.Mana.Value = Mathf.Min(stats.MaxMana.Value, stats.Mana.Value + manaRestoreAmount);
        }

        if (caster != null)
        {
            for (int i = 0; i < caster.spellbook.Count; i++)
            {
                var spell = caster.spellbook[i];
                if (spell == null) continue;
                if (spell.magazineSize > 0)
                {
                    // Reset ammo counts and stop reload coroutine if running
                    caster.currentAmmo[i] = spell.magazineSize;
                    caster.ammoPool[i] = spell.totalAmmo > 0 ? spell.totalAmmo : int.MaxValue;
                    caster.StopAllCoroutines();
                    caster.reloading[i] = false;
                }
            }
        }
    }
}
