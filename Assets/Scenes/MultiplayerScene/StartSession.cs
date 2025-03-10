using UnityEngine;
using Unity.Netcode;

public class StartSession : MonoBehaviour
{
    void Start()
    {
        if (MainPlay.IsHost==1)
        {
            Debug.Log("Starting as Host...");
            NetworkManager.Singleton.StartHost();
        }
        else if(MainPlay.IsHost==2)
        {
            Debug.Log("Starting as Clieeeeent...");
            NetworkManager.Singleton.StartClient();
        }
    }
}
