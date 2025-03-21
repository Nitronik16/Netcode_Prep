using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    Vector3 moveDir = Vector3.zero;

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
            TestServerRpc();

            /*
            randomNumber.Value = new MyCustomData
            {
                _int = 20,
                _bool = false,
                message = "All your base are belong to us!"
            };
            */
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

    [ServerRpc]
    private void TestServerRpc()
    {
        Debug.Log("TestServerRpc " + OwnerClientId);
    }
}
