using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    private Transform playerTransform;
    public Vector2 offset; // Décalage de la caméra
    public float smoothSpeed = 0.125f; // Vitesse de suivi

    private void Start(){
        findPlayer();
    }
    private void Update()
    {
        if (playerTransform == null){
            findPlayer();
            return;
        }
        

        Vector3 desiredPosition = new Vector3(playerTransform.position.x + offset.x, playerTransform.position.y + offset.y, transform.position.z);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
    }

    private void findPlayer(){
        foreach(var player in GameObject.FindGameObjectsWithTag("Player")){
            var networkObject = player.GetComponent<NetworkObject>();
            if(networkObject!=null && networkObject.IsLocalPlayer){
                playerTransform=player.transform;
                break;
            }
        }
    }
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.Escape)){
            SceneManager.LoadSceneAsync(0);
        }
    }

}