using UnityEngine;
using System.Collections;

public abstract class Ability : MonoBehaviour
{
    public string AbilityName;  
    public float cooldown = 1f;  

    protected bool isOnCooldown = false;

    public abstract void Activate();

    // Gere le cooldown automatiquement
    protected IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}
