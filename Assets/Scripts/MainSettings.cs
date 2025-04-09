using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainSettings : MonoBehaviour
{
    public void back(){
        SceneManager.LoadSceneAsync(0);
    }
    public void Keybinds(){
        SceneManager.LoadSceneAsync(4);
    }
    public void Sound()
    {
        
    }
}
