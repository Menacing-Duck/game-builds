using UnityEngine;

[CreateAssetMenu(fileName = "Keybindings", menuName = "Keybindings")]
public class KeyBinds : ScriptableObject
{
    public KeyCode up, down, left, right, interact, inventory, capa1, capa2, capa3, ultimate, settings;
    public KeyCode CheckKey(string key){
        switch(key){
            case "up":
                return up;
            case "down":
                return down;
            case "left":
                return left;
            case "right":
                return right;
            case "interact":
                return interact;
            case "inventory":
                return inventory;
            case "capa1":
                return capa1;
            case "capa2":
                return capa2;
            case "capa3":
                return capa3;
            case "utlimate":
                return ultimate;
            case "settings":
                return settings;
            default:
                return KeyCode.None;
        }

    }
    
}
