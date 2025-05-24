using UnityEngine;
using Unity.Netcode;
using System.Collections;
using UnityEngine.SceneManagement;

public class FlagZone : NetworkBehaviour
{
    public float captureTime = 10f;
    private float currentCaptureTime = 0f;
    private bool isCapturing = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            isCapturing = true;
            currentCaptureTime = 0f;
            StartCoroutine(CaptureRoutine());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            isCapturing = false;
            currentCaptureTime = 0f;
            StopAllCoroutines();
        }
    }

    IEnumerator CaptureRoutine()
    {
        while (isCapturing && currentCaptureTime < captureTime)
        {
            currentCaptureTime += Time.deltaTime;
            yield return null;
        }

        if (isCapturing)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
} 