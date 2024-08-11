using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using TMPro;

public class TCPServer : MonoBehaviour
{
    private TcpListener server;
    private bool isRunning;
    private Thread serverThread;

    public string ipAddress = "192.168.50.243"; // 서버 IP 주소
    public int port = 8081; // 서버 포트 번호
    
    public TextMeshProUGUI Quat_val;
    public TextMeshProUGUI Light_val;
    public Toggle Chk_shutB;
    public Toggle Chk_lightB;

    private string receivedMessage; // 수신된 메시지를 저장할 변수
    private bool messageReceived; // 메시지 수신 플래그
    public GameObject rotateObj;

    private string x, y, z, w, L_pwr, bShutter, bLight = "0";
    public float lerpSpeed = 10f;

    Stopwatch sw = new Stopwatch();    
    void Start()
    {
        StartServer();
    }


    void StartServer()
    {
        server = new TcpListener(IPAddress.Parse(ipAddress), port);
        server.Start();
        isRunning = true;
        serverThread = new Thread(new ThreadStart(Run));
        serverThread.IsBackground = true;
        serverThread.Start();
        Debug.Log("Server started on " + ipAddress + ":" + port);
        //192.168.0.0~255:8081/data
    }

    private void Run()
    {
        while (isRunning)
        {

            try
            {
                // Accept a new client
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Client connected");
                client.NoDelay = true;

                // Create a new thread to handle the client connection
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
            catch (Exception e)
            {
                Debug.Log("Exception: " + e.Message);
            }
        }
    }


    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();

        try
        {
            byte[] buffer = new byte[28]; // 데이터 수신을 위한 버퍼
            int bytesRead; // 수신된 바이트 수
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                    // 수신된 데이터를 문자열로 변환
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received: " + response); // 수신된 데이터를 디버그 로그로 출력
                Thread.Sleep(10);

                // 수신된 메시지와 플래그를 업데이트
                if (response.Length < 3 || response[0] != '$' || response[response.Length - 1] != '#')
                {
                    Debug.LogError(response);
                    Debug.LogError("Invalid format");
                    messageReceived = false;
                }
                else
                {
                    Parsing(response);
                    receivedMessage = response;
                    messageReceived = true;
                }
            }

            // Close the connection
            client.Close();
            Debug.Log("Client disconnected");
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.Message);
        }
        
    }

    void Update()
    {
        // 메시지가 수신되었을 경우 메인 스레드에서 UI 업데이트
        if (messageReceived)
        {
            UpdateOutputText();
            RotateObject();
            messageReceived = false; // 플래그 초기화
        }
        rotateObj.transform.rotation = Quaternion.Lerp(rotateObj.transform.rotation, quaternion, Time.deltaTime * lerpSpeed);

    }

    void UpdateOutputText()
    {
        // 수신된 메시지를 UI 텍스트로 업데이트
        if (Quat_val != null && Light_val != null)
        {
            Quat_val.text = "x: " + x + ", y: " + y + "\nz: " + z + ", w: " + w; // 메시지를 UI 텍스트에 설정
            Light_val.text = "Light Power = " + L_pwr;
        }
        if (bShutter == "1")
            Chk_shutB.isOn = true;
        else
            Chk_shutB.isOn = false;

        if (bLight == "1")
            Chk_lightB.isOn = true;
        else
            Chk_lightB.isOn = false;
    }
    private Quaternion quaternion = Quaternion.identity;
    void RotateObject()
    {
        // 쿼터니언을 오일러 각도로 변환
        // Vector3 eulerAngles = quaternion.eulerAngles;

        // 물체의 회전 설정
        //rotateObj.transform.rotation = Quaternion.Euler(eulerAngles);
        if (float.TryParse(x, out float xValue) && 
            float.TryParse(y, out float yValue) && 
            float.TryParse(z, out float zValue) && 
            float.TryParse(w, out float wValue))
        {            
            quaternion = new Quaternion(xValue, yValue, zValue, wValue);
        }
        else
        {
            Debug.LogError("Invalid float format in quaternion values");
        }
    }
     void Parsing(string input)
    {
        x = y = z = w = L_pwr = bShutter = bLight = string.Empty;

        string[] fields = input.Substring(1, input.Length - 2).Split(',');
        
        
        for (int i = 0; i < fields.Length; i++)
        {
            switch(i)
            {
                case 0:
                    x = fields[i];
                    break;
                case 1:
                    y = fields[i];  
                    break;
                case 2:
                    z = fields[i];
                    break;
                case 3:
                    w = fields[i];
                    break;
                case 4:
                    L_pwr = fields[i];
                    break;
                case 5:
                    bShutter = fields[i];
                    break;
                case 6:
                    bLight = fields[i];
                    break;
                default:
                    break;
            }
        }

        if (fields.Length != 7)
        {
            Debug.LogError("Not enough Fields" + fields.Length);
            return;
        }
    }

    void OnApplicationQuit()
    {
        StopServer();
    }

    void StopServer()
    {
        isRunning = false;
        if (server != null) server.Stop();
        if (serverThread != null) serverThread.Abort();
        Debug.Log("Server stopped");
    }

}
