using UnityEngine;
using System.Collections.Generic;

public class AbilityManager : MonoBehaviour
{
    private List<Ability> Abilities = new List<Ability>();  
    private Ability activeAbility;
    private int currentAbilityIndex = 0;

    void Start()
    {
        // Trouve toutes les capacit�s attach�es au m�me GameObject
        Abilities.AddRange(GetComponents<Ability>());
        if (Abilities.Count > 0)
        {
            SetActiveAbility(0); // S�lectionne la 1ere capacit� au d�but
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Changer de capacite
        {
            CycleAbility();
        }

        if (Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", "F")))) // Activer la capacite numero 1
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
