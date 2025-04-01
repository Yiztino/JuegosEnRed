using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
public class TicTacToeGame : MonoBehaviour
{
    public Button[] buttons; 
    public TMP_Text statusText; 
    private string[] board = new string[9]; 
    private string currentPlayer = "X";
    private int actualTurn;

    void Start()
    {
        ResetGame();
        StartCoroutine(GetText());
        //StartCoroutine(Tirada());
    }

    IEnumerator GetText()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/gato/gato.php?action=2&id=id1");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success/*www.isNetworkError*/)
        {
            Debug.Log(www.error);

        }
        else
        {
            string json = www.downloadHandler.text;
            Debug.Log("JSON recibido: " + json);
            //byte[] results = www.downloadHandler.data;
            TicTacToeData data = JsonUtility.FromJson<TicTacToeData>(json);

            if (data != null && data.board.Length == 9)
            {
                UpdateBoardFromJson(data.board);
            }
        }
    }
    IEnumerator Tirada()
    {
        UnityWebRequest www = UnityWebRequest.Get("http://localhost/gato/gato.php?action=3&id=id1&pos=6");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success/*www.isNetworkError*/)
        {
            Debug.Log(www.error);

        }
        else
        {
            string json = www.downloadHandler.text;
            Debug.Log("JSON recibido: " + json);
            //byte[] results = www.downloadHandler.data;
            //TicTacToeData data = JsonUtility.FromJson<TicTacToeData>(json);

            //if (data != null && data.board.Length == 9)
            //{
            //    UpdateBoardFromJson(data.board);
            //}
        }
    }

    void UpdateBoardFromJson(int[] jsonBoard)
    {
        for (int i = 0; i < jsonBoard.Length; i++)
        {
            if (jsonBoard[i] == 1)
                board[i] = "X";
            else if (jsonBoard[i] == 2)
                board[i] = "O";
            else
                board[i] = ""; // Casilla vacía

            // Actualizar el texto en el botón
            TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = board[i];
            }
        }
    }

    public void OnButtonClick(int index)
    {
        if (board[index] == "") 
        {
            board[index] = currentPlayer;

            TMP_Text buttonText = buttons[index].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = currentPlayer;
            }
            else
            {
                Debug.LogError("Error: El botón no tiene un componente TMP_Text.");
            }

            
            if (CheckWin())
            {
                statusText.text = "Ganador: " + currentPlayer;
                DisableButtons();
                return;
            }

            
            if (CheckTie())
            {
                statusText.text = "¡Empate!";
                DisableButtons();
                return;
            }

            SwitchPlayer();
        }
    }

    void SwitchPlayer()
    {
        currentPlayer = (currentPlayer == "X") ? "O" : "X";
        statusText.text = "Turno: " + currentPlayer;
    }

    bool CheckWin()
    {
        int[,] winPatterns = new int[,]
        {
            {0, 1, 2}, {3, 4, 5}, {6, 7, 8}, // Filas
            {0, 3, 6}, {1, 4, 7}, {2, 5, 8}, // Columnas
            {0, 4, 8}, {2, 4, 6}  // Diagonales
        };

        for (int i = 0; i < winPatterns.GetLength(0); i++)
        {
            if (board[winPatterns[i, 0]] != "" &&
                board[winPatterns[i, 0]] == board[winPatterns[i, 1]] &&
                board[winPatterns[i, 1]] == board[winPatterns[i, 2]])
            {
                return true;
            }
        }
        return false;
    }

    bool CheckTie()
    {
        foreach (string cell in board)
        {
            if (cell == "") return false;
        }
        return true;
    }

    void DisableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }

    public void ResetGame()
    {
        currentPlayer = "X";
        statusText.text = "Turno: X";

        for (int i = 0; i < board.Length; i++)
        {
            board[i] = "";
            TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null) buttonText.text = "";
            buttons[i].interactable = true;
        }
    }
}