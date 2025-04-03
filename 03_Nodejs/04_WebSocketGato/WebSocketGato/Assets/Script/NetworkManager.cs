using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
//using System.Net.WebSockets;

public class NetworkManager : MonoBehaviour
{
    private WebSocket websocket;
    private static string baseURL = "ws://localhost:8080";
    public static NetworkManager Instance;
    public string lastReceivedData;
    public event Action<string> OnDataReceived;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("NetworkManager instanciado.");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Start()
    {
        websocket = new WebSocket(baseURL);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            //Debug.Log("OnMessage!");
            //Debug.Log(bytes);

            // getting the message as a string
            //string message = System.Text.Encoding.UTF8.GetString(bytes);
            //lastReceivedData = message;
            //Debug.Log("OnMessage received: " + message);
            lastReceivedData = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + lastReceivedData);
            OnDataReceived?.Invoke(lastReceivedData);
        };

        // Keep sending messages at every 0.3s
        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue(); // Mandar mensajes sin enviar
#endif
    }

    public async void GetState()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("status");
        }
    }

    public async void Tirada(int idPlayer, int pos)
    {
        if (websocket.State == WebSocketState.Open)
        {
            string message = $"turn:{idPlayer}:{pos}";
            Debug.Log("Enviando tirada: " + message);
            await websocket.SendText(message);
        }
    }

    public async void ResetGame()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("init");
        }
    }
    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}