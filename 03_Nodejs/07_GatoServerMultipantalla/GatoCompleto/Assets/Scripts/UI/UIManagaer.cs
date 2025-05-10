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
    public GridLayoutGroup userList, matchList;
    public GameObject menuInicial, menuStart, playerBtnPrefab, startButton,
        warnNombreOcupado, invitacionRecibidaPopup, invitacionEnviadaPopUp, noEsTuTurnoPopUp;
    public TextMeshProUGUI txtInvitacionRecibida, txtInvitacionEnviada, txtUsernameOnMenu, txtUsernameOnGame;
    public Button startBtn;
    public InputField enterName;
    private int playerNumber;
    private Coroutine actualizarUsuariosCoroutine, actualizarMatchesCoroutine;

    private string matchID;
    private string myUsername;

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
        NetworkManager.Instance.GetMatchesList();
        menuStart.SetActive(false);
        menuInicial.SetActive(true);
        if(actualizarUsuariosCoroutine == null)
        {
            actualizarUsuariosCoroutine = StartCoroutine(ActualizarListaUsuariosPeriodicamente());
        }
        if(actualizarMatchesCoroutine == null)
        {
            actualizarMatchesCoroutine = StartCoroutine(ActualizarListaPartidasPeriodicamente());
        }

    } 
    public void StartGATOGame(string nombreRival, string playernum, string matchID)
    {
        //NetworkManager.Instance.ResetGame();
        if (actualizarUsuariosCoroutine != null)
        {
            StopCoroutine(actualizarUsuariosCoroutine);
            actualizarUsuariosCoroutine = null;
        }
        menuStart.SetActive(false);
        menuInicial.SetActive(false);
        NetworkManager.Instance.currentMatchID = matchID;

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
        NetworkManager.Instance.OnMatchesListReceived += ProcessMatchesListData;
        NetworkManager.Instance.OnGameStartReceived += StartGATOGame;
        NetworkManager.Instance.OnInviteReceived += InviteReceived;
        NetworkManager.Instance.OnErrorReceived += ErrorReceived; ;
        NetworkManager.Instance.OnRejectedReceived += RejectedReceived;

    }
    private void OnDisable()
    {
        NetworkManager.Instance.OnUserDataReceived -= ProcessReceivedData;
        NetworkManager.Instance.OnUsersListReceived -= ProcessUsersListData;
        NetworkManager.Instance.OnMatchesListReceived -= ProcessMatchesListData;
        NetworkManager.Instance.OnInviteReceived -= InviteReceived;
        NetworkManager.Instance.OnGameStartReceived -= StartGATOGame;
        NetworkManager.Instance.OnRejectedReceived -= RejectedReceived;
        NetworkManager.Instance.OnErrorReceived -= ErrorReceived;

    }

    private void ProcessReceivedData(string message, string usuario)
    {
        if (message == "Username updated")
        {
            //myUsername = usuario;
            startButton.SetActive(true);
            ActualizarUsernametxt(usuario);

        }
        else if (message == "Username already in use")
        {
            warnNombreOcupado.SetActive(true);
        }
        else if (message == "Reconnected to your game")
        {
            menuStart.SetActive(false);
            menuInicial.SetActive(false);
            ActualizarUsernametxt(usuario);

        }

    }
    private void ActualizarUsernametxt(string username)
    {
        txtUsernameOnGame.text = $"Tu nombre de usuario: {username}"; 
        txtUsernameOnMenu.text = $"Tu nombre de usuario: {username}"; 
    }
    public void InviteReceived(string usernameInviter)
    {
        invitacionRecibidaPopup.SetActive(true);
        txtInvitacionRecibida.text = $"El usuario: {usernameInviter} te manda invitación de juego";
        myUsername = usernameInviter;
    }
    public void ProcessUsersListData(string userJson)
    {
        UserListData usersList;
        usersList = JsonUtility.FromJson<UserListData>(userJson);
        Debug.Log("Usuarios recibidos: " + string.Join(", ", usersList.users));
        
        foreach (Transform child in userList.transform)
        {
            Destroy(child.gameObject); 
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
    public void ProcessMatchesListData(string matchJson)
    {
        MatchListData matchesList;
        matchesList = JsonUtility.FromJson<MatchListData>(matchJson);
        if (matchesList == null || matchesList.matchID == null)
        {
            Debug.LogWarning("No se pudieron deserializar las partidas o matchesIDs es null. JSON recibido: " + matchJson);
            return;
        }
        Debug.Log("Matches recibidos: " + string.Join(", ", matchesList.matchID));
        
        foreach (Transform child in matchList.transform)
        {
            Destroy(child.gameObject); 
        }

        foreach (string match in matchesList.matchID)
        {
            GameObject playerButton = Instantiate(playerBtnPrefab, matchList.transform);
            TextMeshProUGUI btnText = playerButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = match;
            }

            Button button = playerButton.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnMatchSelected(match));
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
    private void OnMatchSelected(string matchID)
    {
        Debug.Log("Partida seleccionada: " + matchID);
        NetworkManager.Instance.EscogerPartida(matchID);
        NetworkManager.Instance.currentMatchID = matchID;
        menuStart.SetActive(false);
        menuInicial.SetActive(false);
        invitacionEnviadaPopUp.SetActive(false);
        //invitacionEnviadaPopUp.SetActive(true);
        //txtInvitacionEnviada.text = $"Invitación para jugar enviada al usuario: {username}. Por favor espera respuesta";

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
        yield return new WaitForSeconds(1.2f);
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
    IEnumerator ActualizarListaUsuariosPeriodicamente()
    {
        while (true)
        {
            NetworkManager.Instance.GetUsersList();
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator ActualizarListaPartidasPeriodicamente()
    {
        while (true)
        {
            NetworkManager.Instance.GetMatchesList();
            yield return new WaitForSeconds(2f);
        }
    }
    public void AnswerGameInvite(string yesOrNo)
    {
        NetworkManager.Instance.AnswerGameInvite(yesOrNo, myUsername); // Enviamos el username junto con la respuesta
    }
}

