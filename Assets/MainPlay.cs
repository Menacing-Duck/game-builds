using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class MainPlay : MonoBehaviour
{
    public static int IsHost=0; 

    public void back()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void tutorial()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void host()
    {
        IsHost = 1; 
        SceneManager.LoadSceneAsync(5);
    }

    public void join()
    {
        IsHost = 2; 
        SceneManager.LoadSceneAsync(5);
    }
}
