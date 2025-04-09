using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame

    public GameObject PausePanel;
    public GameObject SettingPanel;

    public GameObject KeyBinds;

    public void OnGUI()
    {
        if(SettingPanel.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape)){
            SettingPanel.SetActive(false);
            PausePanel.SetActive(true);
        }
    }

    public void back(){
        SettingPanel.SetActive(false);
        PausePanel.SetActive(true);
    }

    public void keybinds(){
        SettingPanel.SetActive(false);
        KeyBinds.SetActive(true);
    }
}
