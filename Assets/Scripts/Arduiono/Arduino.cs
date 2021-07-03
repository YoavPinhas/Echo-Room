using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;


public class Arduino
{
    private const int ARDUINO_PORT = 8888;
    public String ARDUINO_IP = "10.0.0.2";
    private const int NY_PORT = 8890;
    public String MY_IP = "10.0.0.1";

    private const int NUM_INPUTS = 18;
    private const int NUM_OUTPUTS = 12;

    // holds the state of the arduino's input pins
    private bool[] input_state;
    // holds the state of the arduino's output pins
    private bool[] output_state;

    // PIN INDEX DECLARATIONS - use this instead of the actual pin index
    // example: arduino.set_output_state(Arduino.OUT_RELAY_1, true)
    public const int OUT_RELAY_1 = 0;
    public const int OUT_RELAY_2 = 1;
    // same for input pins
    // example: arduino.get_input_state(Arduino.IN_CABLE_1)
    public const int IN_CABLE_1 = 0;
    public const int IN_CABLE_2 = 1;
    public Func<int, bool, bool> InputChange;

    private UdpClient udpClient;

    // Use this for initialization
      public Arduino(string C_IP, string A_IP)
    {
        MY_IP = C_IP;
        ARDUINO_IP = A_IP;
        input_state = new bool[NUM_INPUTS];
        for (int i=0; i<NUM_INPUTS; i++)
        {
            input_state[i] = false;
        }
        output_state = new bool[NUM_OUTPUTS];
        for (int i=0; i<NUM_OUTPUTS; i++)
        {
            output_state[i] = false;
        }

        IPEndPoint e = new IPEndPoint(IPAddress.Parse(MY_IP), NY_PORT);
        udpClient = new UdpClient(e);
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        //receiveState = new UdpState();
        //receiveState.client = udpClient;
        //receiveState.endpoint = e;


        udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
        
        // initiate the pseudo connection with the arduino
        send_output_state();
    }
    
    public void OnDisable()
    {
        udpClient.Close();
    }

    

    public Arduino()
    {
        input_state = new bool[NUM_INPUTS];
        for (int i=0; i<NUM_INPUTS; i++)
        {
            input_state[i] = false;
        }
        output_state = new bool[NUM_OUTPUTS];
        for (int i=0; i<NUM_OUTPUTS; i++)
        {
            output_state[i] = false;
        }

        IPEndPoint e = new IPEndPoint(IPAddress.Parse(MY_IP), NY_PORT);
        udpClient = new UdpClient(e);

        //receiveState = new UdpState();
        //receiveState.client = udpClient;
        //receiveState.endpoint = e;


        udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);

        // initiate the pseudo connection with the arduino
        send_output_state();
    }

    public void toggle_output_state(int pin_index)
    {
        output_state[pin_index] = !output_state[pin_index];
    }
    public void set_output_state(int pin_index, bool state)
    {
        output_state[pin_index] = state;
    }

    public bool get_input_state(int pin_index)
    {
        return input_state[pin_index];
    }
    
    public void send_output_state()
    {
        
        udpClient.Send(Convert(output_state), output_state.Length, ARDUINO_IP, ARDUINO_PORT);
    }

    private Byte[] Convert(bool[] arr)
    {
        byte[] byteArr = new byte[arr.Length];
        for (int i = 0; i < arr.Length; i++)
        {
            byteArr[i] = (arr[i]) ? (byte)1 : (byte)0;
        }
        return byteArr;
    }
    
    private void ReceiveCallback(IAsyncResult ar)
    {
        
        IPEndPoint e = new IPEndPoint(IPAddress.Parse(ARDUINO_IP), ARDUINO_PORT);
        byte[] receiveBytes = udpClient.EndReceive(ar, ref e);
        if (receiveBytes.Length == input_state.Length)
        {
            for (int i = 0; i < receiveBytes.Length; ++i)
            {
                bool receivedState = receiveBytes[i] != 0;
                if (receivedState != input_state[i])
                {
                    //InputChange.Invoke(i, receivedState);
                    Console.WriteLine($"Received packet with new state at byte {i}: {receivedState}");
                    input_state[i] = receivedState;
                }
            }
        }
        else
        {
            Console.WriteLine($"Received packet with invalid length: {receiveBytes.Length}");
        }

        // initiate another packet receiving
        udpClient.BeginReceive(new AsyncCallback(this.ReceiveCallback), null);
    }


}
