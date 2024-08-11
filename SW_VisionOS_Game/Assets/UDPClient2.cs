using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Diagnostics;

public class UDPClient2 : MonoBehaviour
{
    private UdpClient udpClient;
    private const int serverPort = 8081;
    private const string serverAddress = "192.168.50.25"; // 서버 IP 주소
    private const int clientPort = 8081; // 클라이언트 포트

    void Start()
    {
        try
        {
            udpClient = new UdpClient(clientPort);
            udpClient.Connect(serverAddress, serverPort);
            UnityEngine.Debug.Log($"UDP Client started, connected to {serverAddress}:{serverPort}");
            SendData();
            ReceiveData();
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Exception in UDP Client Start: {ex.Message}");
        }
    }

    private async void SendData()
    {
        while (true)
        {
            try
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string message = $"$0,0,0,0,0,0,0#{timestamp}";
                byte[] data = Encoding.UTF8.GetBytes(message);

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                await udpClient.SendAsync(data, data.Length);
                stopwatch.Stop();

                UnityEngine.Debug.Log($"Sent message: {message}, Round-trip time: {stopwatch.ElapsedMilliseconds} ms");

                if (stopwatch.ElapsedMilliseconds >= 100)
                {
                    UnityEngine.Debug.LogWarning("Communication delay exceeds 100ms");
                }

                await Task.Delay(100); // 100ms 대기
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Exception in SendData: {ex.Message}");
            }
        }
    }

    private async void ReceiveData()
    {
        while (true)
        {
            try
            {
                var result = await udpClient.ReceiveAsync();
                string message = Encoding.UTF8.GetString(result.Buffer);
                UnityEngine.Debug.Log($"Received message: {message}");

                // 메시지를 ','로 분리하여 배열에 저장
                string[] data = message.Trim('#').Split(',');
                UnityEngine.Debug.Log($"Parsed data: {string.Join(", ", data)}");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Exception in ReceiveData: {ex.Message}");
            }
        }
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
