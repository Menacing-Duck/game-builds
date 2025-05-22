using UnityEngine;

public enum AoeSpawnMode { AtMouse, InFrontOfCaster }

[CreateAssetMenu(fileName = "AoeSpell_", menuName = "Spells/AOE")]
public class AoeSpellDefinition : SpellDefinitionBase
{
    public GameObject aoePrefab;
    public AoeSpawnMode spawnMode = AoeSpawnMode.AtMouse;
    public float offsetFromCaster = 1;
    public bool stickToCaster = false;
}
