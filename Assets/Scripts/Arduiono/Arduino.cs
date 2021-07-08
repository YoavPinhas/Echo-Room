using System;
using System.Text;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

public struct ArduinoData
{
    public int channel;
    public bool state;
}
public class Arduino : SingletonMonoBehavior<Arduino>
{
    public bool debug = false;
    private TcpClient socketConnection;
    private Thread clientReciveThread;

    private void Start()
    {
        ConnectToTcpServer();
    }

    private void ConnectToTcpServer()
    {
        try
        {
            clientReciveThread = new Thread(new ThreadStart(ListenForData));
            clientReciveThread.IsBackground = true;
            clientReciveThread.Start();
        }
        catch(Exception e)
        {
            Debug.LogError($"Client connection exception:\n{e}");
        }
    }

    private void ListenForData()
    {
        try
        {
            socketConnection = new TcpClient("localHost", 8080);
            Byte[] bytes = new Byte[1024];
            while (true)
            {
                using(NetworkStream stream = socketConnection.GetStream())
                {
                    int length;

                    while((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        byte[] inData = new byte[length];
                        Array.Copy(bytes, inData, length);
                        string serverMessage = Encoding.ASCII.GetString(inData);
                        ArduinoData data = JsonUtility.FromJson<ArduinoData>(serverMessage);
                        if(debug)
                            Debug.Log($"Recive from arduino: channel = {data.channel}, state = {data.state}");
                    }
                }
            }
        }
        catch(SocketException e)
        {
            Debug.LogError($"Socket exception: {e}");
        }
    }

    public void SendMessage(ArduinoData data)
    {
        if (socketConnection == null)
            return;
        try 
        {
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string output = JsonUtility.ToJson(data);
                
                byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(output);
                
                stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
                if(debug)
                    Debug.Log("A message has been send to arduino.");
            }
        }
        catch(SocketException e)
        {
            Debug.LogError($"Socket exception: {e}");
        }
    }

}
