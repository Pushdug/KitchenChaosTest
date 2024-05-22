using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionUI : MonoBehaviour
{
  private void Start()
  {
    KitchenGameMultiplayer.Instance.OnTryingToJoinGame += KitchenGameMultiplayerOnTryingToJoinGame;
    KitchenGameMultiplayer.Instance.OnFailedToJoinGame += KitchenGameMultiplayerOnFailedToJoinGame;
    
    Hide();
  }

  private void OnDestroy()
  {
    KitchenGameMultiplayer.Instance.OnTryingToJoinGame -= KitchenGameMultiplayerOnTryingToJoinGame;
    KitchenGameMultiplayer.Instance.OnFailedToJoinGame -= KitchenGameMultiplayerOnFailedToJoinGame;
  }

  private void KitchenGameMultiplayerOnFailedToJoinGame (object sender, EventArgs e)
  {
    Hide();
  }

  private void KitchenGameMultiplayerOnTryingToJoinGame (object sender, EventArgs e)
  {
    Show();
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
