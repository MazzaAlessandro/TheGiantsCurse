using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkMovement : NetworkBehaviour
{
    private readonly NetworkVariable<PlayerNetworkData> netState = new(writePerm: NetworkVariableWritePermission.Owner);

    private Vector3 vel;
    private float rotVel;
    [SerializeField] private float cheapInterpolationTime = 0.1f;

    void Update()
    {
        if (IsOwner)
        {
            netState.Value = new PlayerNetworkData()
            {
                position = transform.position,
                rotation = transform.rotation.eulerAngles
            };
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, netState.Value.position, ref vel, cheapInterpolationTime);
            transform.rotation = Quaternion.Euler(0, Mathf.SmoothDampAngle(transform.rotation.eulerAngles.y, netState.Value.rotation.y, ref rotVel, cheapInterpolationTime), 0);
        }
    }

    struct PlayerNetworkData : INetworkSerializable
    {
        private float x, y, z;
        private float yRot;

        internal Vector3 position
        {
            get => new Vector3(x, y, z);
            set
            {
                x = value.x;
                y = value.y;
                z = value.z;
            }
        }

        internal Vector3 rotation
        {
            get => new Vector3(0, yRot, 0);
            set => yRot = value.y;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref x);
            serializer.SerializeValue(ref y);
            serializer.SerializeValue(ref z);

            serializer.SerializeValue(ref yRot);
        }
    }
}
