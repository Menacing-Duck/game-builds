using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject Monster;

    public void spawn()
    {
        Instantiate(Monster, transform.position, transform.rotation);
    }
}
