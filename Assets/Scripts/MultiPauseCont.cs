using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class MultiPlayerController : MonoBehaviour
{

    GameObject PausePanel;
    GameObject SetPanel;
    public void Awake()
    {
        PausePanel = GameObject.FindGameObjectWithTag("Pause");

        SetPanel = GameObject.FindGameObjectWithTag("Settings");
        
    }

    void OnGUI()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && PausePanel.activeInHierarchy == false){
            PausePanel.SetActive(true);
        }
    }

}
