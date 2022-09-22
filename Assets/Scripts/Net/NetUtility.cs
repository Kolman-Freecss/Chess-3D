using System;
using Unity.Networking.Transport;
using UnityEngine;

public enum OpCode
{
    KEEP_ALIVE = 1,
    WELCOME = 2,
    START_GAME = 3,
    MAKE_MOVE = 4,
    REMATCH = 5
}

public static class NetUtility
{
    public static void OnData(DataStreamReader stream, NetworkConnection connection, Server server = null)
    {
        NetMessage msg = null;
        OpCode code = (OpCode)stream.ReadByte();
        Debug.Log("Received NetUtility OpCode: " + code);
        switch (code)
        {
            case OpCode.KEEP_ALIVE:
                msg = new NetKeepAlive(stream);
                break;
            case OpCode.WELCOME:
                msg = new NetWelcome(stream);
                break;
                
            case OpCode.START_GAME:
                msg = new NetStartGame(stream);
                break;
                
            case OpCode.MAKE_MOVE:
                msg = new NetMakeMove(stream);
                break;
            case OpCode.REMATCH:
                msg = new NetRematch(stream);
                break;
            default:
                Debug.Log("Unknown message received");
                break;
        }
        if (server != null)
            msg.ReceivedOnServer(connection);
        else
            msg.ReceivedOnClient();
    }

    // Net messages
    public static Action<NetMessage> C_KEEP_ALIVE;
    public static Action<NetMessage> C_WELCOME;
    public static Action<NetMessage> C_START_GAME;
    public static Action<NetMessage> C_MAKE_MOVE;
    public static Action<NetMessage> C_REMATCH;
    public static Action<NetMessage, NetworkConnection> S_KEEP_ALIVE;
    public static Action<NetMessage, NetworkConnection> S_WELCOME;
    public static Action<NetMessage, NetworkConnection> S_START_GAME;
    public static Action<NetMessage, NetworkConnection> S_MAKE_MOVE;
    public static Action<NetMessage, NetworkConnection> S_REMATCH;
}
