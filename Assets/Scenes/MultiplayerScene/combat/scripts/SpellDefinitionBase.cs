using UnityEngine;

public abstract class SpellDefinitionBase : ScriptableObject
{
    public string spellName = "New Spell";
    public float cooldown = 1;
    public int manaCost = 10;
    public SpellEffect effect;
    public bool affectAllies = false;
    public bool affectEnemies = true;
    public float lifetime = 5;
}
