using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
        public void Back(){
        SceneManager.LoadSceneAsync(3);
    }
    

}
