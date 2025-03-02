using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void playGame(){
        SceneManager.LoadSceneAsync(1);
    }
    public void Settings(){
        SceneManager.LoadSceneAsync(3);
    }
    public void QuitGame(){
        Application.Quit();
    }
}
