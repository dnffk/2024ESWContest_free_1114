using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.Diagnostics;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.Assertions.Must;



public class UDPServer : MonoBehaviour
{
    private UdpClient udpServer;
    private IPEndPoint remoteEndPoint;
    public string serverIp = "192.168.50.243";
    public int serverPort = 8081;
    public TextMeshProUGUI Quat_val;
    public TextMeshProUGUI Light_val;
    public Toggle Chk_shutB;
    public Toggle Chk_lightB;

    private bool messageReceived;
    public GameObject rotateObj;
    public float lerpSpeed;

    private string x, y, z, w, L_pwr, bShutter, bLight = "0";

    Stopwatch sw = new Stopwatch();
    void Start()
    {
        try
        {
            udpServer = new UdpClient(serverPort);
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            StartCoroutine(SendAndReceiveData());
            Application.runInBackground = true; // 백그라운드에서도 실행 가능하게 설정
            Debug.Log("Server Start on IP: "+serverIp+" Port : "+serverPort);

        }
        catch (Exception e)
        {
            Debug.LogError("Server start error: " + e.Message);
        }
    }

    private IEnumerator SendAndReceiveData()
    {
        Debug.Log("S,R Start");
        while (true)
        {
            try
            {
                while (udpServer.Available > 0)
                {
                    sw.Stop();
                    Debug.Log($"Time : {sw.ElapsedMilliseconds}ms");
                    if (sw.ElapsedMilliseconds >= 50)
                        Debug.LogError(sw.ElapsedMilliseconds+"ms");
                    sw.Reset();
                    sw.Start();
                    byte[] receiveData = udpServer.Receive(ref remoteEndPoint);
                    string receivedMessage = Encoding.UTF8.GetString(receiveData);
                    Debug.Log("Received: " + receivedMessage);
                    if (receivedMessage.Length < 3 || receivedMessage[0] != '$' || receivedMessage[receivedMessage.Length - 1] != '#')
                    {
                        Debug.LogError("Invalid format");
                        messageReceived = false;
                    }
                    else
                    {
                        Parsing(receivedMessage);
                        messageReceived = true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Send/Receive data error: " + e.Message);
            }

            yield return new WaitForSeconds(0.001f); // Send message every second
        }
    }
        void UpdateOutputText()
        {
            // 수신된 메시지를 UI 텍스트로 업데이트
            if (Quat_val != null && Light_val != null)
            {
                Quat_val.text = "x: " + x + ", y: " + y + "\nz: " + z + ", w: " + w; // 메시지를 UI 텍스트에 설정
                Light_val.text = "Brightness = " + L_pwr +"%";
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

    private void OnApplicationQuit()
    {
        udpServer.Close();
    }
}