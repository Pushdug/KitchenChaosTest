using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : MonoBehaviour
{
  [SerializeField] private Button playAgainButton;
  
  private void Start()
  {
    NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
    Hide();
  }

  private void NetworkManagerOnClientDisconnectCallback (ulong clientId)
  {
    if (clientId == NetworkManager.ServerClientId)
    {
      Show();
    }
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
