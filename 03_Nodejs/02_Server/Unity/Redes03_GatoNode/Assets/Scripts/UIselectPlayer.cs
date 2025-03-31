using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIselectPlayer : MonoBehaviour
{
    public GameObject menu;
    public int playerNumber; 
    public Button playerButton;
    public static event Action<int> OnPlayerSelected;

    private void Start()
    {
        playerButton.onClick.AddListener(SelectPlayer);
    }

    private void OnDestroy()
    {
        playerButton.onClick.RemoveListener(SelectPlayer);
    }

    private void SelectPlayer()
    {
        //Debug.Log($"Jugador {playerNumber} seleccionado");
        menu.SetActive( false );
        OnPlayerSelected?.Invoke(playerNumber);
    }
}
