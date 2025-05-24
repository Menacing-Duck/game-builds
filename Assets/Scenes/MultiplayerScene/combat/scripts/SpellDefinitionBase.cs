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

    public int magazineSize = 0;
    public int totalAmmo = 0;
    public float reloadTime = 0;
}
