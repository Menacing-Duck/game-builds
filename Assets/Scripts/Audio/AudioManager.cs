using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // On détruit le doublon
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Important !
    }
}

