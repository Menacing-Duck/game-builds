using UnityEngine;
using Unity.Netcode;

public class MonsterDeath : NetworkBehaviour
{
    private Stats stats;
    private NetworkObject networkObject;

    void Start()
    {
        stats = GetComponent<Stats>();
        networkObject = GetComponent<NetworkObject>();
    }

    void Update()
    {
        if (!IsServer) return;

        if (stats != null && stats.health.Value <= 0)
        {
            if (networkObject != null)
            {
                networkObject.Despawn();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
} 