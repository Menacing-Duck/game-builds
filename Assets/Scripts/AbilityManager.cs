using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    private List<Ability> Abilities = new List<Ability>();  
    private Ability activeAbility;
    private int currentAbilityIndex = 0;

    void Start()
    {

        Abilities.AddRange(GetComponents<Ability>());
        if (Abilities.Count > 0)
        {
            SetActiveAbility(0); 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            CycleAbility();
        }

        if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", "F")))) 
        {
            Debug.Log(Abilities[currentAbilityIndex].AbilityName);
            activeAbility?.Activate();
        }
    }

    void CycleAbility()
    {
        currentAbilityIndex = (currentAbilityIndex + 1) % Abilities.Count;
        SetActiveAbility(currentAbilityIndex);
    }

    void SetActiveAbility(int index)
    {
        activeAbility = Abilities[index];
    }
}
