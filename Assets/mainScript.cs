using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using UnityEngine;

public class mainScript : MonoBehaviour
{

    public SerialHandler serialHandler;
    public byte[] value;
    public byte value2 = 100;

    private int time = 0;
    // Start is called before the first frame update
    void Start()
    {
        Array.Resize(ref value, 8);
        Array.Copy(new byte[] { 100, 100, 100, 100, 100, 100, 100, 100 }, 0, value, 0, 8);
        Invoke("DelayMethod", 1.0f);
        //serialHandler.OnDataReceived += serialHandler.SerialDataReceivedEventHandler(moveAmp);
    }

    // Update is called once per frame
    void Update()
    {
        moveAmp();
    }

    private void DelayMethod()
    {
        //Debug.Log("004128" + (char)value + (char)100 + (char)100 + (char)100 + (char)100 + (char)100 + (char)100 + (char)100);
        serialHandler.WriteCAN(0x302, value2);
        Invoke("DelayMethod", 0.1f);
    }

    private void moveAmp()
    {
        //Vector3 position = transform.position; // ローカル変数に格納
        //position.x = float.Parse(serialHandler.DATA); // ローカル変数に格納した値を上書き
        //transform.position = position; // ローカル変数を代入
    }
}