using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JuegoGato : MonoBehaviour
{
    public Button[] buttons;
    public TMP_Text statusText, infoText;
    public int[] board = new int[9];
    public string score1;
    public string score2;
    public int winner;
    public int currentPlayer;
    public string enemyName;
    private bool gameEnded = false;
    public GameObject winText;
    private void OnEnable()
    {
        if (NetworkManager.Instance == null)
        {
            Debug.LogError("NetworkManager.Instance no está inicializado.");
            return;
        }

        NetworkManager.Instance.OnGameDataReceived += ProcessReceivedData;
        NetworkManager.Instance.OnGameStartReceived += UpdateStartUI;
       
    }
    private void OnDisable()
    {
        NetworkManager.Instance.OnGameDataReceived -= ProcessReceivedData;
        NetworkManager.Instance.OnGameStartReceived -= UpdateStartUI;
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
            TextMeshProUGUI txt = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            //TMP_Text buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
            switch (board[i])
            {
                case 0:
                    txt.text = "";
                    break;
                case 1:
                    txt.text = "X";
                    break;
                case 2:
                    txt.text = "O";
                    break;
            }
        }
    }

    private void UpdateInfoOnUI(TicTacToeData data)
    {
        string stringTurn = (data.actual == 1) ? "X" : "O";
        statusText.text = $"Turno de: {stringTurn} | Ronda: {data.round} | Score: {data.score1} - {data.score2}";

    }
    private void UpdateStartUI(string enemyName, string playerNum, string match )
    {
        int playerNumber = int.Parse(playerNum);
        currentPlayer = playerNumber;
        string playerYouAre = (playerNumber == 1) ? "X" : "O";
        infoText.text = $"Te toca jugar con {playerYouAre} contra {enemyName}";
    }

    public void OnButtonClick(int index)
    {

        if (gameEnded) return;
        //Debug.Log("JJJJJJJJJJJJJJJJKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKKRISTELLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL");
        NetworkManager.Instance.Tirada(index);
    }
    private void Win(TicTacToeData data)
    {
        winText.SetActive(true);
        TextMeshProUGUI textoWin = winText.GetComponent<TextMeshProUGUI>();
        string winningText = data.winner == currentPlayer ? "Ganaste!" : "Perdiste";
        textoWin.text = $"{winningText}";
        gameEnded = true;
        

        Debug.Log($"El jugador {data.winner} ganó");
        StartCoroutine(ResetAfterDelay());
        //ResetGame();

    }
    private IEnumerator ResetAfterDelay()
    {

        yield return new WaitForSeconds(2f);
        winText.SetActive(false);
        ResetGame();
        gameEnded = false;
        //NetworkManager.Instance.ResetGam
        //e();
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