using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class PausePanel : MonoBehaviour
{
    public GameObject PausePan;

    public GameObject SetPanel;

    void OnGUI()
    {
        if(PausePan.activeInHierarchy && Input.GetKeyDown(KeyCode.Escape)){
            PausePan.SetActive(false);
        }
    }

    public void Back(){
        PausePan.SetActive(false);
    }

    public void settings()
    {
        SetPanel.SetActive(true);
        PausePan.SetActive(false);
    }

    public void quit()
    {
        Application.Quit();
    } 


}
