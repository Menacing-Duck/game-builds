using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class StartSession : MonoBehaviour
{
    void Start()
    
    {
        Debug.Log(MainPlay.IsHost);
        if (MainPlay.IsHost == 1)
        {
            Debug.Log("Starting as Host...");
            NetworkManager.Singleton.StartHost();
        }
        else if (MainPlay.IsHost == 2)
        {
            Debug.Log("Starting as Clieeeeent...");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(selectUi.dcodedCode, 7777);
            NetworkManager.Singleton.StartClient();

            
        }
    }
}
