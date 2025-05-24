// SpellDefinitionBase.cs
using UnityEngine;

public abstract class SpellDefinitionBase : ScriptableObject
{
    public string spellName = "New Spell";
    public float cooldown = 1;
    public int manaCost = 10;
    public SpellEffect effect;
    public bool affectAllies = true;
    public bool affectEnemies = true;
    public float lifetime = 5;

    public int magazineSize = 0;   // number of shots before reload; 0 = infinite
    public int totalAmmo = 0;      // total bullets available; 0 = unlimited
    public float reloadTime = 0;   // seconds to reload when magazine empty
}
