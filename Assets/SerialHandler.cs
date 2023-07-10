using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{
    public delegate void SerialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e);
    public event SerialDataReceivedEventHandler OnDataReceived;

    //ポート名
    //例
    //Linuxでは/dev/ttyUSB0
    //windowsではCOM1
    //Macでは/dev/tty.usbmodem1421など
    [SerializeField] string portName = "COM1";
    [SerializeField] int baudRate = 9600;

    [SerializeField] private int _CH;

    public int CH { get; protected set; } = 0;
    public int FRAMETYPE = 0;
    public int ID = 0;
    public string ID_HEX;
    public int DLC = 0;
    public byte[] DATA = new byte[8];
    public string DATASTR = "";

    private SerialPort serialPort_;
    private Thread thread_;
    private bool isRunning_ = false;

    private bool isNewMessageReceived_ = false;

    static readonly byte[] hexConvrt = new byte[16] { (byte)'0', (byte)'1', (byte)'2', (byte)'3', (byte)'4', (byte)'5', (byte)'6', (byte)'7',(byte) '8', (byte)'9', (byte)'a', (byte)'b', (byte)'c', (byte)'d',(byte) 'e', (byte)'f'};

    void Awake()
    {
        string[] ports = SerialPort.GetPortNames();

        // 取得したシリアル・ポート名を出力する
        foreach (string port in ports)
        {
            Debug.Log(port);
        }
        Open();
    }

    void Update()
    {
        if (isNewMessageReceived_)
        {
            //SerialDataReceivedEventHandler();
            //Debug.Log(message_);
        }
        isNewMessageReceived_ = false;

        _CH = CH;
    }

    void OnDestroy()
    {
        Close();
    }

    private void Open()
    {
        serialPort_ = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);
        //または
        //serialPort_ = new SerialPort(portName, baudRate);
        serialPort_.ReadTimeout = 50; //[milliseconds]
        serialPort_.Open();

        isRunning_ = true;

        thread_ = new Thread(Read);
        thread_.Start();
    }
    private void Close()
    {
        isNewMessageReceived_ = false;
        isRunning_ = false;

        if (thread_ != null && thread_.IsAlive)
        {
            thread_.Join();
        }

        if (serialPort_ != null && serialPort_.IsOpen)
        {
            serialPort_.Close();
            serialPort_.Dispose();
        }
    }

    private void Read()
    {
        while (isRunning_ && serialPort_ != null && serialPort_.IsOpen)
        {
            try
            {
                byte[] message_ = new byte[17];
                int length = serialPort_.Read(message_, 0, 17);
                Debug.Log(length);
                if (length > 0)
                {
                    /*string temp = "CH:" + message_[0] + " FT:" + message_[1] + " ID:0x" + message_[2] + message_[3] + message_[4] + " DLC:" + message_[5];
                    Debug.Log(temp);
                    */
                    CH = message_[0] - '0';
                    FRAMETYPE = message_[1] - '0';
                    ID_HEX = "0x" + Encoding.ASCII.GetString(message_, 2, 3);
                    ID = Convert.ToInt32(ID_HEX, 16);
                    DLC = message_[5] - '0';

                    DATASTR = Encoding.ASCII.GetString(message_, 6, DLC);
                    Array.Clear(DATA, 0, 8);
                    Array.Copy(message_, 6, DATA, 0, DLC);

                    Debug.Log(Encoding.ASCII.GetString(message_, 0, length));
                    Debug.Log(string.Join(" ", message_.Select(b => b.ToString("X2"))));
                    isNewMessageReceived_ = true;
                }
            }
            catch (System.Exception e)
            {

                Debug.LogWarning(e.Message);

            }
        }
    }


    public enum Channels : byte
    {
        ch1 = 48,
        ch2 = 49
    }

    public enum Frametypes : byte
    {
        data = 48,
        rtr = 49
    }
    /// <summary>
    /// 指定されたCANチャンネルに対してCANフレームを書き込みます。チャンネル、フレームタイプ、アドレス、およびデータを引数として受け取ります。
    /// データは可変長のバイト配列として渡すことができます。ただし、データの要素数は8以下である必要があります。
    /// </summary>
    /// <param name="ch">CANチャンネルを表すChannels列挙型。有効な値はChannels.Channel1またはChannels.Channel2です。</param>
    /// <param name="frametype">フレームの種類を表すFrametypes列挙型。有効な値はFrametypes.dataまたはFrametypes.rtrです。</param>
    /// <param name="address">フレームのアドレスを表す整数値。</param>
    /// <param name="data">フレームのペイロードデータを表す可変長のバイト配列。</param>
    /// <exception cref="System.ArgumentException">渡されたデータの長さが8バイトを超える場合にスローされます。</exception>
    public void WriteCAN(Channels ch, Frametypes frametype, int address, params byte[] data)
    {
        if(data.Length > 8)
        {
            throw new ArgumentException("Too many arguments provided. Please check the expected number of arguments(<=8).");
        }
        byte[] packet = new byte[8 + data.Length];
        packet[0] = 0x02;
        packet[1] = (byte)ch;
        packet[2] = (byte)frametype;
        
        packet[3] = hexConvrt[(address & 0xf00) >> 8];
        packet[4] = hexConvrt[(address & 0x0f0) >> 4];
        packet[5] = hexConvrt[address & 0x00f];

        packet[6] = (byte)(data.Length + '0');

        Array.Copy(data, 0, packet, 7, data.Length);

        packet[packet.Length - 1] = 0x03;

        serialPort_.Write(packet, 0, packet.Length);

    }
    /// <summary>
    /// 指定されたフレームタイプ、アドレス、およびデータを使用してCANフレームを書き込みます。
    /// データは可変長のバイト配列として渡すことができます。ただし、データの要素数は8以下である必要があります。
    /// </summary>
    /// <param name="frametype">フレームの種類を表すFrametypes列挙型。有効な値はFrametypes.dataまたはFrametypes.rtrです。</param>
    /// <param name="address">フレームのアドレスを表す整数値。</param>
    /// <param name="data">フレームのペイロードデータを表す可変長のバイト配列。</param>
    /// <exception cref="System.ArgumentException">渡されたデータの長さが8バイトを超える場合にスローされます。</exception>
    public void WriteCAN(Frametypes frametype, int address, params byte[] data) =>
        WriteCAN(Channels.ch1, frametype, address, data);

    /// <summary>
    /// 指定されたアドレスとデータを使用してCANフレームを書き込みます。
    /// 可変長のバイト配列としてデータを引数として受け取ります。ただし、データの要素数は8以下である必要があります。
    /// </summary>
    /// <param name="address">フレームのアドレスを表す整数値。</param>
    /// <param name="data">フレームのペイロードデータを表す可変長のバイト配列。</param>
    /// <exception cref="System.ArgumentException">渡されたデータの長さが8バイトを超える場合にスローされます。</exception>
    public void WriteCAN(int address, params byte[] data) =>
        WriteCAN(Channels.ch1, Frametypes.data, address, data);
}