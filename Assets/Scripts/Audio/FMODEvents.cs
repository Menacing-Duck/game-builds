using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{
    [SerializeField] private EventReference musicEvent;

    private EventInstance musicInstance;

    void Start()
    {
        musicInstance = RuntimeManager.CreateInstance(musicEvent);
        musicInstance.start();
    }

    private void OnDestroy()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        musicInstance.release();
    }
}