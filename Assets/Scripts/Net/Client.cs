using System;
using Unity.Networking.Transport;
using UnityEngine;

public class Client : MonoBehaviour
{
    #region Singleton implementation
    public static Client Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public NetworkDriver driver;
    private NetworkConnection connection;

    private bool isActive = false;
    
    public Action connectionDropped;

    // Methods
    public void Init(string ip, ushort port)
    {
        driver = NetworkDriver.Create();
        NetworkEndPoint endpoint = NetworkEndPoint.Parse(ip, port);

        connection = driver.Connect(endpoint);

        Debug.Log("Connecting to " + ip + ":" + port);

        isActive = true;

        RegisterToEvent();
    }

    public void Shutdown()
    {
        if (isActive)
        {
            UnregisterFromEvent();
            driver.Dispose();
            isActive = false;
            connection = default(NetworkConnection);
        }
    }

    private void OnDestroy() 
    {
        Shutdown();
    }


    private void Update() 
    {
        if (!isActive)
            return;

        driver.ScheduleUpdate().Complete();
        CheckAlive();
        UpdateMessagePump();
    }

    private void CheckAlive()
    {
        if (!connection.IsCreated && isActive)
        {
            Debug.Log("Connection dropped");
            connectionDropped?.Invoke();
            Shutdown();
        }
    }

    private void UpdateMessagePump()
    {
        DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = connection.PopEvent(driver, out stream)) != NetworkEvent.Type.Empty)
        {
            switch (cmd)
            {
                case NetworkEvent.Type.Connect:
                    Debug.Log("Connected to server");
                    //SendToServer(new NetWelcome());
                    break;
                case NetworkEvent.Type.Data:
                    //HandleData(stream);
                    break;
                case NetworkEvent.Type.Disconnect:
                    Debug.Log("Disconnected from server");
                    connection = default(NetworkConnection);
                    connectionDropped?.Invoke();
                    Shutdown();
                    break;
            }
        }
    }

    public void SendToServer(NetMessage msg)
    {
        DataStreamWriter writer;
        driver.BeginSend(connection, out writer);
        //msg.Serialize(ref writer);
        driver.EndSend(writer);
    }

    //Event parsing
    private void RegisterToEvent()
    {
        throw new NotImplementedException();
    }

    private void UnregisterFromEvent()
    {
        throw new NotImplementedException();
    }

    private void OnKeepAlive(NetMessage msg)
    {
        SendToServer(msg);
    }


}
