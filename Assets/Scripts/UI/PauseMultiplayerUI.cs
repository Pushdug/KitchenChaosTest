using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{
  private void Start()
  {
    GameManager.Instance.OnMultiplayerGamePaused += GameManagerOnMultiplayerGamePaused;
    GameManager.Instance.OnMultiplayerGameUnpaused += GameManagerOnMultiplayerGameUnpaused;
    
    Hide();
  }

  private void GameManagerOnMultiplayerGameUnpaused (object sender, EventArgs e)
  {
    Hide();
  }

  private void GameManagerOnMultiplayerGamePaused (object sender, EventArgs e)
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