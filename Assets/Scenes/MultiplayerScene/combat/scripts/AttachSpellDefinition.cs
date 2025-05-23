using UnityEngine;

[CreateAssetMenu(fileName = "AttachSpell_", menuName = "Spells/Attach")]
public class AttachSpellDefinition : SpellDefinitionBase
{
    public GameObject attachPrefab;
    public float offsetFromCenter = 0;
}
