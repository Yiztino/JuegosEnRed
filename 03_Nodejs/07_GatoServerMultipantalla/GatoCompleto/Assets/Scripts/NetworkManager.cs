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
    public event Action<string> OnGameDataReceived;
    public event Action<string,string> OnUserDataReceived;
    public event Action<string> OnUsersListReceived;
    public event Action<string> OnMatchesListReceived;
    public event Action<string, string, string> OnGameStartReceived;
    public event Action<string> OnInviteReceived;
    public event Action<string> OnErrorReceived;
    public event Action<string> OnRejectedReceived;

    public string currentMatchID;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //Debug.Log("NetworkManager instanciado.");
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
           
            lastReceivedData = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received: " + lastReceivedData);

            string[] splitArray = lastReceivedData.Split(char.Parse("|")); //Here we assing the splitted string to array by that char
                                                               //info = splitArray[0];
            if (splitArray[0] == "message")
            {
                Debug.Log($"Message received: {splitArray[1]}");
                OnUserDataReceived?.Invoke(splitArray[1], splitArray[2]);
            }
            else if (splitArray[0] == "data")
            {
                Debug.Log($"Data received: {splitArray[1]}");
                OnGameDataReceived?.Invoke(splitArray[1]);
            }
            else if (splitArray[0] == "usersList")
            {
                Debug.Log($"Data received: {splitArray[1]}");
                OnUsersListReceived?.Invoke(splitArray[1]);
            }
            else if (splitArray[0] == "matchesList")
            {
                OnMatchesListReceived?.Invoke(splitArray[1]);
            }
            else if (splitArray[0] == "START_GAME")
            {
                //Debug.Log($"Data received: {splitArray[1]}");
                OnGameStartReceived?.Invoke(splitArray[1], splitArray[2], splitArray[3]);
            }
            else if (splitArray[0] == "invite")
            {
                //Debug.Log($"Data received: {splitArray[1]}");
                OnInviteReceived?.Invoke(splitArray[1]);
            }
            else if (splitArray[0] == "error")
            {
                OnErrorReceived?.Invoke(splitArray[1]);
            }
            else if (splitArray[0] == "REJECTED")
            {
                OnRejectedReceived?.Invoke(splitArray[1]);
            }
           
            //OnDataReceived?.Invoke(lastReceivedData);
        };

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
            await websocket.SendText($"status|{currentMatchID}");
        }
    }

    public async void Tirada(int pos)
    {
        Debug.Log("SIQUIERA ENVIO Enviando tirada: " );
        if (websocket.State == WebSocketState.Open && !string.IsNullOrEmpty(currentMatchID))
        {
            string message = $"turn|{currentMatchID}|{pos}";
            Debug.Log("Enviando tirada: " + message);
            await websocket.SendText(message);
        }
    }

    public async void ResetGame()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText($"resetBoard|{currentMatchID}|");
        }
    }
    public async void UpdateUsername(string name)
    {
        if (websocket.State == WebSocketState.Open)
        {
            
            string message = $"updateUsername|{name}";
            await websocket.SendText(message);

        }
    }
    public async void GetUsersList()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("getUsersList|");
        }
    }
    public async void GetMatchesList()
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText("getMatchesList|");
        }
    }
    public async void SendGameInvite(string username)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText($"sendGameInvite|{username}");
        }
    }
    public async void EscogerPartida(string partidID)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText($"gameSelected|{partidID}");
        }
    }
    public async void AnswerGameInvite(string yesOrNo, string username)
    {

        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText($"gameInviteResponse|{yesOrNo}|{username}");
        }
    }
    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}