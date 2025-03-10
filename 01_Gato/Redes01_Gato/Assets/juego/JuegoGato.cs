using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoGato : MonoBehaviour
{
    public Button[] buttons;
    public TMP_Text statusText;
    private int round = 1;
    public int[] board = new int[9];
    public string score1;
    public string score2;
    public int winner;
    private int currentPlayer;
    public float timeToRefresh = 5f;

    public void Start()
    {
        ResetGame();
        StartCoroutine(GetBoardStatusRepeatedly(timeToRefresh));

    }
    public IEnumerator GetBoardStatusRepeatedly(float interval)
    {
        while (true)
        {
            yield return StartCoroutine(NetworkManager.Instance.GetState());
           
            ProcessReceivedData();
            
            
            yield return new WaitForSeconds(interval);
        }
    }
    private void ProcessReceivedData()
    {
        string json = NetworkManager.Instance.lastReceivedData;

        if (!string.IsNullOrEmpty(json))
        {
            Debug.Log("Procesando datos: " + json);
            TicTacToeData data = JsonUtility.FromJson<TicTacToeData>(json);
            if (data != null && data.board.Length == 9)
            {
                UpdateBoard(data);
                UpdateInfoOnUI(data);
                currentPlayer = data.actual;
                //Debug.Log("Current player con data actual: " + currentPlayer);
                if (data.winner != 0)
                {
                    Win(data);
                }
            }
        }
    }
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
        }
    }
   
    private void UpdateInfoOnUI(TicTacToeData data)
    {
        string stringTurn = (data.actual == 1) ? "X" : "O";
        statusText.text = $"Turno: {stringTurn} | Ronda: {data.round} | Score: {data.score1} - {data.score2 }"  ;
        
    }

    public void OnButtonClick(int index)
    {
        if (currentPlayer == 0) // Agregar verificación para evitar clics antes de que el jugador sea válido
        {
            Debug.LogError("Error: No se ha asignado un jugador válido");
            return;
        }
        //Debug.Log("CurrentPlayer en click" + currentPlayer);
        //int adjustedIndex = index - 1;
        StartCoroutine(NetworkManager.Instance.Tirada(currentPlayer, index));
        StartCoroutine(NetworkManager.Instance.GetState());

    }
    private void Win(TicTacToeData data)
    {

        Debug.Log($"El jugador {data.winner} ganó");
        ResetGame();

    }
    private void ResetGame()
    {
        StartCoroutine(NetworkManager.Instance.ResetGame());
        //string json = NetworkManager.Instance.lastReceivedData;

        ////currentPlayer = 1;
        ////statusText.text = "Turno de X"; 
        ////statusText.text = $"Turno: {stringTurn} | Ronda: {data.round} | Score: {data.score1} - {data.score2}";
        ////string stringTurn = (data.actual == 1) ? "X" : "O";
        ////statusText.text = $"Turno: {stringTurn} | Ronda: {data.round} | Score: {data.score1} - {data.score2}";
        //for (int i = 0; i < board.Length; i++)
        //{
        //    board[i] = 0;
        //    TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
        //    if (buttonText != null) buttonText.text = "";
        //    buttons[i].interactable = true;
        //}

        StartCoroutine(NetworkManager.Instance.ResetGame());
    }
    //if (board[adjustedIndex] == 0)
    //{
    //    board[adjustedIndex] = currentPlayer;


    //    TMP_Text buttonText = buttons[adjustedIndex].GetComponentInChildren<TMP_Text>();
    //    if (buttonText != null)
    //    {
    //        buttonText.text = (currentPlayer == 1) ? "X" : "O";
    //    }

    //    StartCoroutine(NetworkManager.Instance.Tirada(currentPlayer, adjustedIndex));

    //    if (CheckWinner())
    //    {
    //        Win();
    //    }
    //    else if (CheckTie())
    //    {
    //        Empate();
    //    }
    //    else
    //    {
    //        SwitchPlayer();
    //    }
    //}
    //solo por si acaso:
}
//private bool CheckWinner()
//{
//    int[,] winPatterns = new int[,]
//    {
//    {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Filas
//    {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Columnas
//    {0, 4, 8}, {2, 4, 6}  // Diagonales
//    };

//    for (int i = 0; i < winPatterns.GetLength(0); i++)
//    {
//        int a = winPatterns[i, 0];
//        int b = winPatterns[i, 1];
//        int c = winPatterns[i, 2];

//        if (board[a] != 0 && board[a] == board[b] && board[a] == board[c])
//        {   
//            winner = board[a];
//            return true;
//        }
//    }
//        return false;
//}

//void SwitchPlayer()
//{
//    currentPlayer = (currentPlayer == 1) ? 0 : 1;
//}
//void DisableButtons()
//{
//    foreach (Button button in buttons)
//    {
//        button.interactable = false;
//    }
//}

    
    //private void Empate()
    //{
    //    Debug.Log("Fue un empate");
    //}

