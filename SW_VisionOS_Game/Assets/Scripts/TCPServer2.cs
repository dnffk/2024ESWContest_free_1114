using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Collections;
public class TCPServer2 : MonoBehaviour
{
    private NetworkStream stream;
    private static TCPServer2 instanace = null;
    public static TCPServer2 Instance
    { get { return instanace; } }
    private TcpListener server;
    private Thread serverThread;
    private TcpClient connectedTcpClient;
    public TextMeshProUGUI Quat_val;
    public TextMeshProUGUI Light_val;
    public Toggle Chk_shutB;
    public Toggle Chk_lightB;
    public string ipAddress = "192.168.50.176";
    public int port = 8081;
    private bool isRunning;
    private bool messageReceived; // 메시지 수신 플래그
    private GameObject rotateObj;
    public string x, y, z, w, L_pwr, bShutter, bLight = "0";
    public float lerpSpeed = 10f;
    int hitGhost = 0;
    int count = 0;
    private int countAngle = 0;
    private Quaternion quaternion;
    private Quaternion initialRotation;
    private Quaternion previousRotation;
    private Quaternion currentRotation;
    private Quaternion correctedQuaternion;
    private Quaternion smoothQuaternion;
    private int countShut;
    Stopwatch sw = new Stopwatch();
    private bool isHitGhostSending = false;

    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (instanace == null)
        {
            instanace = this;
            DontDestroyOnLoad(instanace);  // 씬이 전환되어도 유지
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        isRunning = true;
        serverThread = new Thread(new ThreadStart(Run));
        serverThread.IsBackground = true;
        serverThread.Start();
        Debug.Log("Server started on " + GetLocalIPAddress() + ":" + port);
    }
    public void chkHitGhost(int value)
    {
        Debug.Log("hitghost 호출");
        hitGhost = value;

        // hitGhost가 설정되면 코루틴 실행

        Debug.Log("hitghost 호출 후 값 할당됨");
    }
    private void Run()
    {
        while (isRunning)
        {
            try
            {
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("Client connected");
                client.NoDelay = true;
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
            catch (Exception e)
            {
                Debug.Log("Exception : " + e.Message);
            }
        }
    }
    private void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        stream = client.GetStream();
        try
        {
            byte[] buffer = new byte[256]; // 데이터 수신을 위한 버퍼
            int bytesRead; // 수신된 바이트 수
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                Debug.Log("Handle Start");
                // 수신된 데이터를 문자열로 변환
                sw.Stop();
                string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received: " + response); // 수신된 데이터를 디버그 로그로 출력
                                                    // 수신된 메시지와 플래그를 업데이트
                Debug.Log("Handle 1");
                if (response.Length < 3 || response[0] != '$' || response[response.Length - 1] != '#')
                {
                    Debug.LogError("Invalid format");
                    messageReceived = false;
                }
                else
                {
                    Debug.Log("Handle 2");
                    string[] fields2 = response.Split('\r');
                    Debug.Log("Handle 3");
                    Parsing(fields2[0]);
                    Debug.Log("Handle 4");
                    if (fields2.Length > 1)
                    {
                        for (int i = 1; i < fields2.Length - 1; i++)
                        {
                            Debug.Log("Splited data: " + fields2[i]);
                            Parsing(fields2[i]);
                        }
                    }
                    Debug.Log("Handle 5");
                    messageReceived = true;
                }
                if (hitGhost != 0)
                {
                    StartCoroutine(SendHitGhost(client));
                }
                //if (hitGhost != 0)
                //{
                //    string data = "$" + hitGhost + "#\r";
                //    Debug.Log("hit Ghost value  : " + hitGhost);
                //    Send(client, data);
                //    Debug.Log("call send hit Ghost");
                //    hitGhost = 0;
                //    Send(client, "$0#\r");
                //} 
                //byte[] ack = Encoding.ASCII.GetBytes("ACK");
                //stream.Write(ack, 0, ack.Length);
                //Debug.Log("ACK sent");
                Debug.Log($"Time : {sw.ElapsedMilliseconds}ms");
                sw.Reset();
                sw.Start();
                if (!client.Connected)
                {
                    Debug.Log("Client disconnected. Trying to reconnect...");
                    Reconnect(client);
                }
                Debug.Log("Handle Last");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception : " + e.Message);
        }
    }
    private void Reconnect(TcpClient client)
    {
        try
        {
            client.Close();
            // 재연결 로직 추가
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            Debug.Log("Reconnected to client");
        }
        catch (Exception e)
        {
            Debug.LogError("Reconnection failed: " + e.Message);
            // 재연결이 실패하면 다시 재연결을 시도하거나 종료 처리
        }
    }



    private IEnumerator SendHitGhost(TcpClient client)
    {
        // 1초 동안 hitGhost 값을 전송
        float timer = 0f;
        while (timer < 1f)
        {
            string data = "$" + hitGhost + "#\r";
            Debug.Log("hit Ghost value  : " + hitGhost);
            Send(client, data);
            Debug.Log("call send hit Ghost : "+data);

            timer += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        // 1초 후 hitGhost를 0으로 설정하고 클라이언트로 전송
        hitGhost = 0;
        Send(client, "$0#\r");
    }


    void Update()
    {
        //UpdateOutputText();
        rotateObj = GameObject.FindWithTag("GameCamera");
        Debug.Log("카메라 회전값 : " + rotateObj.transform.rotation);
        // 메시지가 수신되었을 경우 메인 스레드에서 UI 업데이트
        if (messageReceived)
        {
            RotateObject();
            messageReceived = false; // 플래그 초기화
        }

        if (countShut >= 20)
        {
            initialRotation = Camera.main.transform.rotation; // (0, 0, 0)으로 초기화
            rotateObj.transform.rotation = initialRotation;
            countShut = 0;
        }
        /*
        Quaternion adjustedRotation = Quaternion.Inverse(initialRotation) * quaternion;
        Vector3 currentRotation = adjustedRotation.eulerAngles;
        currentRotation.x = -currentRotation.x;
        adjustedRotation = Quaternion.Euler(currentRotation);
        Debug.Log("조절된 카메라 회전값 : " + adjustedRotation);
        rotateObj.transform.rotation = Quaternion.Lerp(rotateObj.transform.rotation, adjustedRotation, Time.deltaTime * lerpSpeed);
        Debug.Log("현재 카메라 회전값" + rotateObj.transform.rotation);
        if (countShut >= 20)
        {
            initialRotation = Quaternion.identity; // (0, 0, 0)으로 초기화
            rotateObj.transform.rotation = initialRotation;
            countShut = 0;
        }
        */
    }
    void UpdateOutputText()
    {
        // 수신된 메시지를 UI 텍스트로 업데이트
        //if (Quat_val != null && Light_val != null)
        //{
        Debug.Log("Light1 : " + L_pwr);
        ValueManager.Instance.Set_Lightness_Value(float.Parse(L_pwr));
        Debug.Log("Light2 : " + L_pwr);
        //}
        if (bShutter == "1")
        {
            Debug.Log("셔터 입력 들어;");
            ValueManager.Instance.Set_Check_shutButton(1);
            countShut++;
        }
        else
        {
            Debug.Log("셔터 입력 안들어;");
            ValueManager.Instance.Set_Check_shutButton(0);
        }
        if (bLight == "1")
        {

            Debug.Log("손전등 입력 들어옴");
            ValueManager.Instance.Set_Check_lightButton(1);
        }
        else
        {
            /*
            count++;
            if (count == 2)
            {*/
            ValueManager.Instance.Set_Check_lightButton(0);
            count = 0;
            Debug.Log("손전등 입력 안 들어옴");
        }
    }
    void RotateObject()
    {
        Debug.Log("각도변환 ");
        // 받은 쿼터니언 값을 검증하고 변환
        if (float.TryParse(x, out float xValue) &&
            float.TryParse(y, out float yValue) &&
            float.TryParse(z, out float zValue) &&
            float.TryParse(w, out float wValue))
        {
            //quaternion = new Quaternion(wValue, xValue, zValue, yValue);
            //quaternion = new Quaternion(xValue, zValue, yValue, wValue);
            quaternion = new Quaternion(-xValue, zValue, yValue, wValue);
            //quaternion = new Quaternion(xValue, -yValue, zValue, -wValue);
            //quaternion = new Quaternion(xValue, -zValue, yValue, -wValue);
            //quaternion = new Quaternion(-xValue, zValue, yValue, -wValue);
            //quaternion = Quaternion.Inverse(quaternion);
            //quaternion = new Quaternion(xValue, -zValue, -yValue, wValue);  
            //quaternion = Quaternion.Inverse(quaternion);
        }
        else
        {
            Debug.LogError("Invalid float format in quaternion values");
            return;
        }
        // 초기 회전값을 (0, 0, 0)으로 설정
        if (countAngle <= 0)
        {

            initialRotation = Quaternion.identity; // (0, 0, 0)으로 초기화
            rotateObj.transform.rotation = initialRotation;
            previousRotation = quaternion;
            countAngle++; // 초기화 이후에는 더 이상 실행되지 않도록 카운트 증가
        }

        /*
        Vector3 myEulerAngles = QimuLowerArm.eulerAngles; 
        Quaternion invertedRotation = Quaternion.Euler(myEulerAngles.x, -myEulerAngles.y, myEulerAngles.z);                 
        transform.localRotation = invertedRotation;
        */

        /*
        currentRotation = quaternion;
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(previousRotation);
        rotateObj.transform.rotation = Quaternion.Slerp(rotateObj.transform.rotation, rotateObj.transform.rotation * deltaRotation, Time.deltaTime * 100f);
        previousRotation = currentRotation;
        Debug.Log("Euler: " + rotateObj.transform.rotation.eulerAngles);
        Debug.Log("현재 카메라 회전값" + rotateObj.transform.rotation);
        */
        currentRotation = quaternion;
        Quaternion deltaRotation = currentRotation * Quaternion.Inverse(previousRotation);
        rotateObj.transform.rotation = Quaternion.Slerp(rotateObj.transform.rotation, rotateObj.transform.rotation * deltaRotation, Time.deltaTime * 100f);
        previousRotation = currentRotation;
        Debug.Log("Euler: " + rotateObj.transform.rotation.eulerAngles);
        Debug.Log("현재 카메라 회전값" + rotateObj.transform.rotation);
    }
    void Parsing(string input)
    {
        x = y = z = w = L_pwr = bShutter = bLight = string.Empty;
        string[] fields = input.Substring(1, input.Length - 2).Split(',');
        for (int i = 0; i < fields.Length; i++)
        {
            switch (i)
            {
                case 0:
                    z = fields[i];
                    break;
                case 1:
                    y = fields[i];
                    break;
                case 2:
                    x = fields[i];
                    break;
                case 3:
                    w = fields[i];
                    break;
                case 4:
                    L_pwr = fields[i];
                    break;
                case 5:
                    bLight = fields[i];
                    break;
                case 6:
                    bShutter = fields[i];
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
        Debug.Log("텍스트 업데이/");
        UpdateOutputText();
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
    string GetLocalIPAddress()
    {
        string localIP = "";
        foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
    public void Send(TcpClient client, string data)
    {
        try
        {
            Debug.Log("call Send");
            if (client != null && client.Connected)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                stream.Write(buffer, 0, buffer.Length);
                Debug.Log("Data sent to client: " + data);
            }
            else
                Debug.Log("데이터 전송 실패 ");
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send data to client: " + e.Message);
        }
    }
}