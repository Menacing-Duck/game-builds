using UnityEngine;
using Unity.Netcode;

public class CharacterSpawnManager : NetworkBehaviour
{
    [Header("Drag your character prefabs here (must match selectUI selection indices)")]
    public GameObject[] characterPrefabs;

    [Header("Drag your spawn-point Transforms here")]
    public Transform[] spawnPoints;

    public override void OnNetworkSpawn()
    {
        // Only run on the server side when a new client connects
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null && IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        // Ask the client what their selection was
        SpawnCharacterRequestClientRpc(new ClientSpawnRequest { ClientId = clientId }, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
        });
    }

    // A tiny struct to carry the RPC data
    public struct ClientSpawnRequest : INetworkSerializable
    {
        public ulong ClientId;
        public int Selection;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ClientId);
            serializer.SerializeValue(ref Selection);
        }
    }

    /// <summary>
    /// Sent to the client to retrieve its static selectUi.selection and echo back to the server.
    /// </summary>
    [ClientRpc]
    private void SpawnCharacterRequestClientRpc(ClientSpawnRequest requestData, ClientRpcParams rpcParams = default)
    {
        // Populate the selection from the static selectUi class
        requestData.Selection = selectUi.selection;
        // Send it back to the server
        ConfirmSpawnServerRpc(requestData);
    }

    /// <summary>
    /// Actually runs on the server to instantiate the right prefab at the right spawn point.
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    private void ConfirmSpawnServerRpc(ClientSpawnRequest requestData, ServerRpcParams serverParams = default)
    {
        int sel = requestData.Selection;
        ulong clientId = requestData.ClientId;

        // Safety checks
        if (sel < 0 || sel >= characterPrefabs.Length || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"[SpawnManager] Invalid selection or no spawn points!");
            return;
        }

        // Simple round-robin assignment of spawn points
        int spawnIndex = (int)(clientId % (ulong)spawnPoints.Length);

        Vector3 pos = spawnPoints[spawnIndex].position;
        Quaternion rot = spawnPoints[spawnIndex].rotation;

        GameObject go = Instantiate(characterPrefabs[sel], pos, rot);
        var netObj = go.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId, true);
        
        Debug.Log($"[SpawnManager] Spawned client {clientId} with character #{sel} at spawn #{spawnIndex}");
    }
}
