using UnityEngine;
using System.Net.Sockets; // TCP 소켓 통신을 위한 네임스페이스
using System.Text; // 문자열 인코딩을 위한 네임스페이스
using System.Threading; // 쓰레드 사용을 위한 네임스페이스
using TMPro; // TextMeshPro 사용을 위한 네임스페이스
using UnityEngine.UI;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.Rendering;
using System;
using UnityEngine.Analytics; // UI 사용을 위한 네임스페이스

public class TCPClient2 : MonoBehaviour
{
    private TcpClient client; // TCP 클라이언트를 위한 변수
    private NetworkStream stream; // 네트워크 스트림을 위한 변수
    private Thread clientThread; // 클라이언트 통신을 위한 쓰레드

    public string serverIp = "192.168.50.25"; // 서버 IP 주소
    public int serverPort = 8081; // 서버 포트 번호
    public TextMeshProUGUI Quat_val;
    public TextMeshProUGUI Light_val;
    public Toggle Chk_shutB;
    public Toggle Chk_lightB;

    private string receivedMessage; // 수신된 메시지를 저장할 변수
    private bool messageReceived; // 메시지 수신 플래그
    public GameObject rotateObj;

    private string x, y, z, w, L_pwr, bShutter, bLight = "0";
    public float lerpSpeed = 10f;

    void Start()
    {
        // 새로운 쓰레드를 생성하여 서버에 연결
        clientThread = new Thread(new ThreadStart(ConnectToServer));
        clientThread.IsBackground = true; // 백그라운드 쓰레드로 설정
        clientThread.Start(); // 쓰레드 시작
    }

    void ConnectToServer()
    {
        while (true)
        {
            try
            {
                client = new TcpClient(serverIp, serverPort); // 서버에 연결
                client.NoDelay = true;
                stream = client.GetStream(); // 네트워크 스트림 초기화
                Debug.Log("Connect to server");

                byte[] buffer = new byte[256]; // 데이터 수신을 위한 버퍼
                int bytesRead; // 수신된 바이트 수

                // 데이터 수신 루프
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    // 수신된 데이터를 문자열로 변환
                    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Debug.Log("Received: " + response); // 수신된 데이터를 디버그 로그로 출력

                    // 수신된 메시지와 플래그를 업데이트
                    if (response.Length < 3 || response[0] != '$' || response[response.Length - 1] != '#')
                    {
                        Debug.LogError("Invalid format");
                        messageReceived = false;
                    }
                    else
                    {
                        Parsing(response);
                        receivedMessage = response;
                        messageReceived = true;
                    }

                    byte[] ack = Encoding.ASCII.GetBytes("ACK");
                    stream.Write(ack, 0, ack.Length);
                    Debug.Log("ACK sent");
                }
            }
            catch (SocketException e)
            {
                // 소켓 예외 발생 시 디버그 로그 출력
                Debug.Log("SocketException: " + e);
            }
            finally
            {
                // 스트림과 클라이언트 닫기
                if (stream != null) stream.Close();
                if (client != null) client.Close();
            }
            Debug.Log("Retrying connect in 5 seconds. . .");
            Thread.Sleep(5000);
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
        // 애플리케이션 종료 시 스트림과 클라이언트 닫기
        if (stream != null) stream.Close();
        if (client != null) client.Close();
        if (clientThread != null) clientThread.Abort(); // 쓰레드 중단
    }

    
}
