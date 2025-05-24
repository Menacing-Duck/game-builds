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

        SpawnCharacterRequestClientRpc(new ClientSpawnRequest { ClientId = clientId }, new ClientRpcParams
        {
            Send = new ClientRpcSendParams { TargetClientIds = new[] { clientId } }
        });
    }


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


    [ClientRpc]
    private void SpawnCharacterRequestClientRpc(ClientSpawnRequest requestData, ClientRpcParams rpcParams = default)
    {

        requestData.Selection = selectUi.selection;

        ConfirmSpawnServerRpc(requestData);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ConfirmSpawnServerRpc(ClientSpawnRequest requestData, ServerRpcParams serverParams = default)
    {
        int sel = requestData.Selection;
        ulong clientId = requestData.ClientId;


        if (sel < 0 || sel >= characterPrefabs.Length || spawnPoints.Length == 0)
        {
            Debug.LogWarning($"[SpawnManager] Invalid selection or no spawn points!");
            return;
        }


        int spawnIndex = (int)(clientId % (ulong)spawnPoints.Length);

        Vector3 pos = spawnPoints[spawnIndex].position;
        Quaternion rot = spawnPoints[spawnIndex].rotation;

        GameObject go = Instantiate(characterPrefabs[sel], pos, rot);
        var netObj = go.GetComponent<NetworkObject>();
        netObj.SpawnAsPlayerObject(clientId, true);
        
        Debug.Log($"[SpawnManager] Spawned client {clientId} with character #{sel} at spawn #{spawnIndex}");
    }
}
