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
        warnNombreOcupado, invitacionRecibidaPopup, invitacionEnviadaPopUp, noEsTuTurnoPopUp;
    public TextMeshProUGUI txtInvitacionRecibida, txtInvitacionEnviada;
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
    public void StartGATOGame(string nombreRival, string playernum)
    {
        //NetworkManager.Instance.ResetGame();

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
        NetworkManager.Instance.OnInviteReceived += InviteReceived;
        NetworkManager.Instance.OnErrorReceived += ErrorReceived; ;
        NetworkManager.Instance.OnRejectedReceived += RejectedReceived;

    }
    private void OnDisable()
    {
        NetworkManager.Instance.OnUserDataReceived -= ProcessReceivedData;
        NetworkManager.Instance.OnUsersListReceived -= ProcessUsersListData;
        NetworkManager.Instance.OnInviteReceived -= InviteReceived;
        NetworkManager.Instance.OnGameStartReceived -= StartGATOGame;
        NetworkManager.Instance.OnRejectedReceived -= RejectedReceived;
        NetworkManager.Instance.OnErrorReceived -= ErrorReceived;

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
        else if (message == "Reconnected to your game")
        {
            menuStart.SetActive(false);
            menuInicial.SetActive(false);
        }
        
    }
    public void InviteReceived(string usernameInviter)
    {
        invitacionRecibidaPopup.SetActive(true);
        txtInvitacionRecibida.text = $"El usuario: {usernameInviter} te manda invitación de juego";
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
        invitacionEnviadaPopUp.SetActive(true);
        txtInvitacionEnviada.text = $"Invitación para jugar enviada al usuario: {username}. Por favor espera respuesta";

    }
    private void ErrorReceived(string message)
    {
        if(message == "no es tu turno")
        {
            StartCoroutine(NoEsTuTurnoPopUp());
        }
    }
    IEnumerator NoEsTuTurnoPopUp()
    {
        noEsTuTurnoPopUp.SetActive(true);
        yield return new WaitForSeconds(2f);
        noEsTuTurnoPopUp.SetActive(false);
    }
    private void RejectedReceived(string usuario)
    {
        Debug.Log("ESTO OCURRE??????????????????????????????????????/");
    
        StartCoroutine(InvitacionRechazada(usuario));
    }
    IEnumerator InvitacionRechazada(string usuarioQRechazo)
    {
        txtInvitacionEnviada.text = $"El usuario {usuarioQRechazo} rechazó tu invitación";
        yield return new WaitForSeconds(2f);
        invitacionEnviadaPopUp.SetActive(false);
    }
}

