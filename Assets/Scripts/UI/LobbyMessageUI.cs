using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
  [SerializeField] private TextMeshProUGUI messageText;
  [SerializeField] private Button closeButton;

  private void Awake()
  {
    closeButton.onClick.AddListener(Hide);
  }

  private void Start()
  {
    KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayerOnFailedToJoinGame;
    KitchenGameLobby.Instance.OnCreateLobbyStarted += KitchenGameLobbyOnCreateLobbyStarted;
    KitchenGameLobby.Instance.OnCreateLobbyFailed += KitchenGameLobbyOnCreateLobbyFailed;
    KitchenGameLobby.Instance.OnJoinStarted += KitchenGameLobbyOnJoinStarted;
    KitchenGameLobby.Instance.OnQuickJoinFailed += KitchenGameLobbyOnQuickJoinFailed;
    KitchenGameLobby.Instance.OnJoinFailed += KitchenGameLobbyOnJoinFailed;
    
    
    Hide();
  }

  private void KitchenGameLobbyOnJoinFailed (object sender, EventArgs e)
  {
    ShowMessage("Failed to join Lobby");
  }

  private void KitchenGameLobbyOnQuickJoinFailed (object sender, EventArgs e)
  {
    ShowMessage("Couldn't find a Lobby to quick join");
  }

  private void KitchenGameLobbyOnJoinStarted (object sender, EventArgs e)
  {
    ShowMessage("Joining Lobby...");
  }

  private void KitchenGameLobbyOnCreateLobbyFailed (object sender, EventArgs e)
  {
    ShowMessage("Failed to create Lobby");
  }

  private void KitchenGameLobbyOnCreateLobbyStarted (object sender, EventArgs e)
  {
    ShowMessage("Creating Lobby...");
  }

  private void OnDestroy()
  {
    KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayerOnFailedToJoinGame;
  }

  private void KitchenGameMultiplayerOnFailedToJoinGame (object sender, EventArgs e)
  {
    ShowMessage(NetworkManager.Singleton.DisconnectReason == "" ? "Failed to connect" : NetworkManager.Singleton.DisconnectReason);
  }

  private void ShowMessage(string message)
  {
    Show();
    messageText.text = message;
  }

  private void Show()
  {
    gameObject.SetActive(true);
  }
  
  private void Hide()
  {
    gameObject.SetActive(false);
  }
}
