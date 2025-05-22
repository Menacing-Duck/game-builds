using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileSpell_", menuName = "Spells/Projectile")]
public class ProjectileSpellDefinition : SpellDefinitionBase
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 10;
    public bool  isHoming       = false;
    public float homingStrength = 2;
    public float homingDelay    = 0;

    public bool  automaticFire  = false;
}
