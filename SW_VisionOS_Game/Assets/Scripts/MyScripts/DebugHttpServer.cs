using UnityEngine;
using System.Net;
using System.Text;
using System.Threading;
public class DebugHttpServer : MonoBehaviour
{
    private HttpListener listener;
    private Thread serverThread;
    private ValueManager valueManager;
    void Start()
    {
        valueManager = ValueManager.Instance;
        listener = new HttpListener();
        // 모든 IP 주소에서 수신을 허용
        listener.Prefixes.Add("http://*:8080/");
        listener.Start();
        serverThread = new Thread(HandleRequests);
        serverThread.Start();
        Debug.Log("HTTP server started at http://*:8080/");
    }
    void HandleRequests()
    {
        while (listener.IsListening)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            string responseString = valueManager.LoadDebugData() ?? "{\"error\": \"No data available\"}";
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
        }
    }
    void OnApplicationQuit()
    {
        listener.Stop();
        serverThread.Abort();
    }
}