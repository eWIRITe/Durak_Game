using WebSocketSharp;
using UnityEngine;

public class Server_controller : MonoBehaviour
{
    private WebSocket webSocket;
    public string serverUrl = "ws://example.com:8080"; // Замените на реальный URL вашего WebSocket-сервера
    public string roomId = "room1"; // Замените на реальный ID комнаты

    private void Start()
    {
        webSocket = new WebSocket(serverUrl);

        webSocket.OnOpen += OnWebSocketOpen;
        webSocket.OnMessage += OnWebSocketMessage;
        webSocket.OnClose += OnWebSocketClose;
        webSocket.OnError += OnWebSocketError;

        webSocket.Connect();

        // Отправьте сообщение с информацией о комнате при подключении
        SendMessageToServer($"Connected to room: {roomId}");
    }

    private void OnWebSocketOpen(object sender, System.EventArgs e)
    {
        Debug.Log("WebSocket connection opened");
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        string message = e.Data;
        Debug.Log($"Received message from server: {message}");

        // Обработка входящих сообщений от сервера
    }

    private void OnWebSocketClose(object sender, CloseEventArgs e)
    {
        Debug.Log("WebSocket connection closed");
    }

    private void OnWebSocketError(object sender, ErrorEventArgs e)
    {
        Debug.Log($"WebSocket error: {e.Message}");
    }

    private void SendMessageToServer(string message)
    {
        if (webSocket != null && webSocket.ReadyState == WebSocketState.Open)
        {
            webSocket.Send(message);
        }
    }

    // ...
}
