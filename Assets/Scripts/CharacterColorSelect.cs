using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelect : MonoBehaviour
{
  [SerializeField] private int colorId;
  [SerializeField] private Image image;
  [SerializeField] private GameObject selectedGameObject;
  
  private void Awake()
  {
   GetComponent<Button>().onClick.AddListener((() =>
   {
     KitchenGameMultiplayer.Instance.ChangePlayerColor(colorId);
   }));  
  }
  
  private void Start()
  {
    KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged += KitchenGameMultiplayerOnPlayerDataNetworkListChanged;
    image.color = KitchenGameMultiplayer.Instance.GetPlayerColor(colorId);
    UpdateIsSelected();
  }

  private void KitchenGameMultiplayerOnPlayerDataNetworkListChanged (object sender, EventArgs e)
  {
    UpdateIsSelected();
  }

  private void UpdateIsSelected()
  {
    selectedGameObject.SetActive(KitchenGameMultiplayer.Instance.GetPlayerData().colorId == colorId);
  }

  private void OnDestroy()
  {
    KitchenGameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= KitchenGameMultiplayerOnPlayerDataNetworkListChanged;
  }
}
