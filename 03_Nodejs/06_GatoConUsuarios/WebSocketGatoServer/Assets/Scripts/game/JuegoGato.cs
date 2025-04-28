using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoGato : MonoBehaviour
{
    public Button[] buttons;
    public TMP_Text statusText;
    public int[] board = new int[9];
    public string score1;
    public string score2;
    public int winner;
    public int currentPlayer;



    //private void OnEnable()
    //{
    //    UIselectPlayer.OnPlayerSelected += SetCurrentPlayer;
    //    NetworkManager.Instance.OnDataReceived += ProcessReceivedData;
    //}
    private void OnEnable()
    {
        // Comprobar si la instancia de NetworkManager existe
        if (NetworkManager.Instance == null)
        {
            Debug.LogError("NetworkManager.Instance no está inicializado.");
            return;
        }

        NetworkManager.Instance.OnGameDataReceived += ProcessReceivedData;
       
    }
    private void OnDisable()
    {
        NetworkManager.Instance.OnGameDataReceived -= ProcessReceivedData;
    }
   

    public void StartGame()
    {
        ResetGame();
        NetworkManager.Instance.GetState();
    }
    
    private void ProcessReceivedData(string json)
    {
        if (!string.IsNullOrEmpty(json))
        {
            //TicTacToeData data = JsonUtility.FromJson<TicTacToeData>(json);
            //if (data != null && data.board.Length == 9)
            //{
            //    UpdateBoard(data);
            //    UpdateInfoOnUI(data);

            //    if (data.winner != 0)
            //    {
            //        Win(data);
            //    }
            //}
            //Debug.Log("Datos recibidos del servidor: " + json);
            TicTacToeData data;
            try
            {
                data = JsonUtility.FromJson<TicTacToeData>(json);
            }
            catch
            {
                Debug.LogError("Error al deserializar el JSON recibido: " + json);
                return;
            }

            if (data != null && data.board.Length == 9)
            {
                Debug.Log("Datos recibidos correctamente: " + json);
                UpdateBoard(data);
                UpdateInfoOnUI(data);

                if (data.winner != 0)
                {
                    Win(data);
                }
            }
        }
    }
   
    //}
    private void UpdateBoard(TicTacToeData data)
    {
        for (int i = 0; i < data.board.Length; i++)
        {
            board[i] = data.board[i];
            TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = (board[i] == 1) ? "X" : (board[i] == 2) ? "O" : "";
            }
            buttons[i].interactable = board[i] == 0;
        }
    }

    private void UpdateInfoOnUI(TicTacToeData data)
    {
        string stringTurn = (data.actual == 1) ? "X" : "O";
        statusText.text = $"Turno: {stringTurn} | Ronda: {data.round} | Score: {data.score1} - {data.score2}";

    }

    public void OnButtonClick(int index)
    {
        if (currentPlayer == 0)
        {
            Debug.LogError("Error: No se ha asignado un jugador válido");
            return;
        }

        NetworkManager.Instance.Tirada(currentPlayer, index);
    }
    private void Win(TicTacToeData data)
    {

        Debug.Log($"El jugador {data.winner} ganó");
        ResetGame();

    }
    private void ResetGame()
    {
        NetworkManager.Instance.ResetGame();

        for (int i = 0; i < board.Length; i++)
        {
            board[i] = 0;
            TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null) buttonText.text = "";
            buttons[i].interactable = true;
        }
        //StartCoroutine(NetworkManager.Instance.ResetGame());
    }
}