using UnityEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Security.Cryptography;
using TMPro;
using System.ComponentModel;

public class KeysPanel : MonoBehaviour
{
    public GameObject KeyB;

    public GameObject Sett;

    public Dictionary<string, KeyCode> Keys = new Dictionary<string, KeyCode>();

    public TextMeshProUGUI up, down, left, right, interact, inventory, capa1, capa2, capa3, ultimate, pause;
    private Color32 normal = new Color32(255,255,255,255);

    private Color32 Clicked = new Color32(172,172,172,172);


    public GameObject CurrentKey;
    void Start(){
        Keys.Add("up",(KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("up", "W")));
        Keys.Add("down", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("down", "S")));
        Keys.Add("left", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", "Q")));
        Keys.Add("right", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", "D")));
        Keys.Add("interact", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", "F")));
        Keys.Add("inventory", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("inventory", "E")));
        Keys.Add("capa1", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("capa1", "F")));
        Keys.Add("capa2", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("capa2", "G")));
        Keys.Add("capa3", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("capa3", "C")));
        Keys.Add("ultimate", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("ultimate", "X")));
        Keys.Add("pause", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("pause", "escape")));
        up.text = Keys["up"].ToString();
        down.text = Keys["down"].ToString();
        left.text = Keys["left"].ToString();
        right.text = Keys["right"].ToString();
        interact.text = Keys["interact"].ToString();
        inventory.text = Keys["inventory"].ToString();
        capa1.text = Keys["capa1"].ToString();
        capa2.text = Keys["capa2"].ToString();
        capa3.text = Keys["capa3"].ToString();
        ultimate.text = Keys["ultimate"].ToString();
        pause.text = Keys["pause"].ToString();
        SaveKeys();
    }

    void OnGUI()
    {
        if(CurrentKey != null){
            Event e = Event.current;
            if(e.isKey){
                Keys[CurrentKey.name] = e.keyCode;
                CurrentKey.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = e.keyCode.ToString();
                CurrentKey.GetComponent<Image>().color = normal;
                CurrentKey = null;
            }
        }

        if(KeyB.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape)){
            KeyB.SetActive(false);
            Sett.SetActive(true);
        }
    }
    public void SaveKeys(){
        foreach(var Key in Keys){
            PlayerPrefs.SetString(Key.Key, Key.Value.ToString());
        }
        PlayerPrefs.Save();
    }
    public void ChangeKey(GameObject clicked){
        if(CurrentKey != null){
            CurrentKey.GetComponent<Image>().color = normal;
        }
        CurrentKey = clicked;
        CurrentKey.GetComponent<Image>().color = Clicked;
    }

    public void back(){
        KeyB.SetActive(false);
        Sett.SetActive(true);
    }
}

