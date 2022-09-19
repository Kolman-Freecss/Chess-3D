using Unity.Networking.Transport;
using UnityEngine;

public class NetMessage : MonoBehaviour
{
    public OpCode Code { get; set; }

    public virtual void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
    }

    public virtual void Deserialize(ref DataStreamReader reader)
    {
        Code = (OpCode)reader.ReadByte();
    }

    public virtual void ReceivedOnClient()
    {
        Debug.Log("Received on client");
    }

    public virtual void ReceivedOnServer(NetworkConnection connection)
    {
        Debug.Log("Received on server");
    }

}
