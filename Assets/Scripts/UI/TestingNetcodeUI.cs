using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TestingNetcodeUI : MonoBehaviour
{
   [SerializeField] private Button startHostButton;
   [SerializeField] private Button startClientButton;

    private void Awake()
    {
        startHostButton.onClick.AddListener(() => {
            Debug.Log("Host");
            KitchenGameMultiplayer.Instance.StartHost();
            gameObject.SetActive(false);
        });

        startClientButton.onClick.AddListener(() => {
            Debug.Log("Client");
            KitchenGameMultiplayer.Instance.StartClient();
            gameObject.SetActive(false);
        });
    }


}
