using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour
{
  private void Start()
  {
    GameManager.Instance.OnLocalPlayerReadyChanged += GameManagerOnLocalPlayerReadyChanged;
    GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
    
    Hide();
  }

  private void GameManagerOnStateChanged (object sender, EventArgs e)
  {
    if (GameManager.Instance.isCountDownToStartActive())
    {
      Hide();
    }
  }

  private void GameManagerOnLocalPlayerReadyChanged (object sender, EventArgs e)
  {
    if (GameManager.Instance.IsLocalPlayerReady())
    {
      Show();
    }
  }

  private void Hide()
  {
    gameObject.SetActive(false);
  }  
  
  private void Show()
  {
    gameObject.SetActive(true);
  }
}
