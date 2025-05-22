using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{

    public GameObject Monster;


    public void spawn()
    {
        Instantiate(Monster, transform.position, transform.rotation);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
