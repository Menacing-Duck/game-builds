// ProjectileSpellDefinition.cs
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
    public int   pierceCount    = 0;

    public bool  isChargeable    = false;
    public float maxChargeTime   = 2f;
    public float minSpeedPercent = 0.5f;
    public float maxSpeedPercent = 2f;

    public float minDamagePercent = 1f;
    public float maxDamagePercent = 2f;
}
