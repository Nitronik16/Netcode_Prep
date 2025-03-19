using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    Vector3 moveDir = Vector3.zero;

    private void Update()
    {
        //need to add this to the movement scripts
        if (!IsOwner) return;

        if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
        else if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
        else moveDir.z = 0f;

        if (Input.GetKey(KeyCode.A)) moveDir.x = +1f;
        else if (Input.GetKey(KeyCode.D)) moveDir.x = -1f;
        else moveDir.x = 0f;

        float moveSpeed = 3f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }
}
