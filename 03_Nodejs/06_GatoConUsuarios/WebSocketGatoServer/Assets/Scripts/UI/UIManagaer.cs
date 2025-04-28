using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;
using System;
using UnityEditor;
using System.Reflection;

public class UIManagaer : MonoBehaviour
{
    public GridLayoutGroup userList;
    public GameObject menuInicial, menuStart, playerBtnPrefab, startButton,
        warnNombreOcupado, invitacionRecibidaPopup;
    public TextMeshProUGUI txtInvitacionRecibida;
    public Button startBtn;
    public InputField enterName;
    private int playerNumber;



    void Start()
    {
        startBtn.onClick.AddListener(StartGame);
    }
    void OnDestroy()
    {
        startBtn.onClick.RemoveListener(StartGame);
    }

    private void StartGame()
    {
        NetworkManager.Instance.GetUsersList();
        menuStart.SetActive(false);
        menuInicial.SetActive(true);

    } 
    public void StartGATOGame(string nombreRIval)
    {
        NetworkManager.Instance.ResetGame();

        menuStart.SetActive(false);
        menuInicial.SetActive(false);

    }
    public void RejectInvite()
    {
        
        menuStart.SetActive(false);
        menuInicial.SetActive(true);
        invitacionRecibidaPopup.SetActive(false);

    }
    private void OnEnable()
    {
        NetworkManager.Instance.OnUserDataReceived += ProcessReceivedData;
        NetworkManager.Instance.OnUsersListReceived += ProcessUsersListData;
        NetworkManager.Instance.OnGameStartReceived += StartGATOGame;

    }
    private void OnDisable()
    {
        NetworkManager.Instance.OnUserDataReceived -= ProcessReceivedData;
        NetworkManager.Instance.OnGameStartReceived -= StartGATOGame;

    }

    private void ProcessReceivedData(string message, string usuario)
    {
        if (message == "Username updated")
        {
            startButton.SetActive(true);
        }
        else if (message == "Username already in use")
        {
            warnNombreOcupado.SetActive(true);
        }
        else if (message == "Invitation Received")
        {
            invitacionRecibidaPopup.SetActive(true);
            txtInvitacionRecibida.text = $"El usuario: {usuario} ten manda invitación de juego";
        }
    }

    public void ProcessUsersListData(string userJson)
    {
        UserListData usersList;
        usersList = JsonUtility.FromJson<UserListData>(userJson);
        Debug.Log("Usuarios recibidos: " + string.Join(", ", usersList.users));
        
        foreach (Transform child in userList.transform)
        {
            Destroy(child.gameObject); // Limpiamos la lista antes de agregar nuevos botones
        }

        foreach (string username in usersList.users)
        {
            GameObject playerButton = Instantiate(playerBtnPrefab, userList.transform);
            TextMeshProUGUI btnText = playerButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = username;
            }

            Button button = playerButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnPlayerSelected(username));
            }
        }
    }
    private void OnPlayerSelected(string username)
    {
        Debug.Log("Usuario seleccionado: " + username);
        NetworkManager.Instance.SendGameInvite(username);
    }
}
    
    //for (int i = 0; i<spritesForAllies.Length; i++)
    //    {
    //        GameObject allyBtn = Instantiate(allyBtnPrefab, tiendaGridLayout.transform);
    //allyBtn.GetComponent<Image>().sprite = spritesForAllies[i];
    //        Button abtn = allyBtn.GetComponent<Button>();
    //TextMeshProUGUI txtBtn = allyBtn.GetComponentInChildren<TextMeshProUGUI>();
    ////print("GameUICanvasManager texto del boton es: " + txtBtn);
    //int index = i;
    //Vector2 btnPosition = allyBtn.transform.position;
    //txtBtn.text = $"${preciosDeAliados[i]}";
    //        //print("GameUICanvasManager texto del precio por boton es: " + txtBtn.text);
    //        abtn.onClick.AddListener(() => AliadoSeleccionado(index, btnPosition));
//    //    }
//    private void SendGameInvite()
//    {

//    }
//    //private void SelectPlayer()
//    //{
//    //    //Debug.Log($"Jugador {playerNumber} seleccionado");
//    //    menu.SetActive(false);
//    //    OnPlayerSelected?.Invoke(playerNumber);
//    //}
//}
