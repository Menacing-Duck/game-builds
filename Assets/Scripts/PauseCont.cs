using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    public GameObject PausePanel;

    public GameObject SetPanel;

    void OnGUI()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && PausePanel.activeInHierarchy == false){
            PausePanel.SetActive(true);
        }
    }

}
