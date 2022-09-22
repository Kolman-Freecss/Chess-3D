using Unity.Networking.Transport;

public class NetRematch : NetMessage
{
    public int teamId;
    public byte wantRematch;

    public NetRematch()
    {
        Code = OpCode.REMATCH;
    }

    public NetRematch(DataStreamReader stream)
    {
        Code = OpCode.REMATCH;
        Deserialize(stream);
    }

    public override void Serialize(ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)Code);
        writer.WriteInt(teamId);
        writer.WriteByte(wantRematch);
    }

    public override void Deserialize(DataStreamReader reader)
    {
        teamId = reader.ReadInt();
        wantRematch = reader.ReadByte();
    }

    public override void ReceivedOnClient()
    {
        NetUtility.C_REMATCH?.Invoke(this);
    }

    public override void ReceivedOnServer(NetworkConnection connection)
    {
        NetUtility.S_REMATCH?.Invoke(this, connection);
    }
}
