using UnityEngine;

[CreateAssetMenu(fileName = "DashSpell_", menuName = "Spells/Dash")]
public class DashSpellDefinition : SpellDefinitionBase
{
    public float dashDistance = 3;
    public float dashTime     = 0.1f;
    public bool  ignoreCollisions = true;
}
