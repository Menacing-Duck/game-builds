using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainPathfinding : MonoBehaviour
{
    public List<GameObject> Players;

    public bool IsInRange;

    public GameObject ClosePlayer;

    
    public MainPathfinding(List<GameObject> players){
        Players = players;
        IsInRange = false;
        ClosePlayer = Players[0];
    }

    public void search(){
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float shortestDistance = Mathf.Infinity;
        GameObject nearest = null;

        foreach (GameObject player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearest = player;
            }
        }

        ClosePlayer = nearest;
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
