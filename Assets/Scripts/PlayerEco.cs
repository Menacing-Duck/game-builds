using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerEco : MonoBehaviour
{
    public int Money;

    public int CurLvl;
    public int Hp;

    public int Mana;

    public int MaxMana;

    public bool IsMed;

    public PlayerEco(bool ismed){
        Money = 10;
        Hp = 50;
        Mana = 10;
        MaxMana = 20;
        IsMed = ismed;
        CurLvl = 1;
    }

}
