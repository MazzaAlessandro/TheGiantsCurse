using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private NetworkVariable<Vector3> netPos = new(writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> netRot = new (writePerm: NetworkVariableWritePermission.Owner);

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            netPos.Value = transform.position;
            netRot.Value = transform.rotation;
        }
        else
        {
            transform.position = netPos.Value;
            transform.rotation = netRot.Value;
        }
    }
}
