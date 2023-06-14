using System;
using Unity.Collections;
using Unity.Netcode;

public struct LobbyPlayerState : INetworkSerializable, IEquatable<LobbyPlayerState>{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;

    public LobbyPlayerState(ulong clientId, FixedString32Bytes playerName){
        ClientId = clientId;
        PlayerName = playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter{
        serializer.SerializeValue(ref ClientId);
        serializer.SerializeValue(ref PlayerName);
    }

    public bool Equals(LobbyPlayerState other){
        return ClientId == other.ClientId &&
            PlayerName.Equals(other.PlayerName);
    }
}
