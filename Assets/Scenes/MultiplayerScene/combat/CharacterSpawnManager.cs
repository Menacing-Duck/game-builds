using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterSpawnManager : MonoBehaviour
{
    public List<GameObject> playerPrefabs = new();
    public List<Transform>  spawnPoints  = new();  // assign in inspector
    Dictionary<ulong, byte> choice = new();

    void Awake()
    {
        var nm = NetworkManager.Singleton;
        nm.ConnectionApprovalCallback    = Approval;
        nm.OnClientConnectedCallback    += OnClientConnected;
    }

    public void SetChoiceForLocal(byte index)
    {
        var nm = NetworkManager.Singleton;
        if (nm.IsHost) choice[nm.LocalClientId] = index;
        nm.NetworkConfig.ConnectionData = new[] { index };
    }

    void Approval(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        byte idx = req.Payload.Length > 0 ? req.Payload[0] : (byte)0;
        choice[req.ClientNetworkId] = idx;
        res.Approved            = true;
        res.CreatePlayerObject = false;
    }

    void OnClientConnected(ulong clientId)
    {
        if (!NetworkManager.Singleton.IsServer) return;

        byte idx   = choice.TryGetValue(clientId, out var v) ? v : (byte)0;
        int  slot  = Mathf.Clamp(idx, 0, playerPrefabs.Count - 1);

        // choose spawn‐point:
        Vector3 spawnPos = Vector3.zero;
        if (spawnPoints.Count > 0)
        {
            // simple round‐robin by clientId:
            int pt = (int)(clientId % (ulong)spawnPoints.Count);
            spawnPos = spawnPoints[pt].position;
        }

        SpawnPrefabFor(clientId, slot, spawnPos);
    }

    public void SpawnPrefabFor(ulong clientId, int prefabIndex, Vector3 pos)
    {
        int i = Mathf.Clamp(prefabIndex, 0, playerPrefabs.Count - 1);
        var go = Instantiate(playerPrefabs[i], pos, Quaternion.identity);
        go.GetComponent<NetworkObject>()
          .SpawnAsPlayerObject(clientId, true);
    }
}
