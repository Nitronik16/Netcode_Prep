using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    Vector3 moveDir = Vector3.zero;

    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;

    private NetworkVariable<MyCustomData> randomNumber = 
        new NetworkVariable<MyCustomData>(
            new MyCustomData
            {
                _int = 20,
                _bool = true,
            }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
        {
            Debug.Log(OwnerClientId + "; " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
        };
    }

    private void Update()
    {
        Debug.Log(OwnerClientId + "; randomNumber: "+ randomNumber.Value);

        //need to add this to the movement scripts
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.T))
        {

            spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);


            //TestClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } });

            /*
            randomNumber.Value = new MyCustomData
            {
                _int = 20,
                _bool = false,
                message = "All your base are belong to us!"
            };
            */
        }

        if (Input.GetKeyDown(KeyCode.Y) && spawnedObjectTransform != null)
        {
            spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
        }

        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        else if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        else moveDir.z = 0f;

        if (Input.GetKey(KeyCode.A)) moveDir.x = +1f;
        else if (Input.GetKey(KeyCode.D)) moveDir.x = -1f;
        else moveDir.x = 0f;

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    //clients send a message to the server/host 
    [ServerRpc]
    private void TestServerRpc(ServerRpcParams serverRpcParams)
    {
        Debug.Log("TestServerRpc " + OwnerClientId + "; " + serverRpcParams.Receive.SenderClientId);
    }

    //server/hos sends a message to all/specified clients 
    [ClientRpc]
    private void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log("TestClientRpc");
    }
}
