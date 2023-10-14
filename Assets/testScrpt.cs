using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO.Ports;

public class testScrpt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SerialPort mySerialPort = new SerialPort("COM5");

        mySerialPort.BaudRate = 9600;
        mySerialPort.Parity = Parity.None;
        mySerialPort.StopBits = StopBits.One;
        mySerialPort.DataBits = 8;
        mySerialPort.Handshake = Handshake.None;
        mySerialPort.RtsEnable = false;

        mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

        mySerialPort.Open();

        Debug.Log("Press any key to continue...");

    }
    private static void DataReceivedHandler(
                    object sender,
                    SerialDataReceivedEventArgs e)
    {
        SerialPort sp = (SerialPort)sender;
        string indata = sp.ReadExisting();
        Debug.Log(indata);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
