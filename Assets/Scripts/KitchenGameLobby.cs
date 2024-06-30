using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Random = UnityEngine.Random;

public class KitchenGameLobby : MonoBehaviour
{
  public static KitchenGameLobby Instance { get; private set;}

  public event EventHandler OnCreateLobbyStarted;
  public event EventHandler OnCreateLobbyFailed;
  public event EventHandler OnJoinStarted;
  public event EventHandler OnQuickJoinFailed;
  public event EventHandler OnJoinFailed;
  public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
  public class OnLobbyListChangedEventArgs : EventArgs
  {
    public List<Lobby> lobbyList;
  }

  private Lobby joinedLobby;
  private float heartBeatTimer;
  private float listLobbiesTimer;
  
  private void Awake()
  {
    Instance = this;
    
    DontDestroyOnLoad(gameObject);
    InitializeUnityAuthentication();
  }

  private async void InitializeUnityAuthentication()
  {
    if (UnityServices.State != ServicesInitializationState.Initialized)
    {
      InitializationOptions initializationOptions = new InitializationOptions();
      initializationOptions.SetProfile(Random.Range(0, 10000).ToString());
      
      await UnityServices.InitializeAsync(initializationOptions);
      await AuthenticationService.Instance.SignInAnonymouslyAsync(); 
      
    }
  }

  private void Update()
  {
    HandleHeartbeat();
    HandlePeriodicListLobbies();
  }

  private void HandlePeriodicListLobbies()
  {
    if (joinedLobby == null && AuthenticationService.Instance.IsSignedIn)
    {
      listLobbiesTimer -= Time.deltaTime;

      if (listLobbiesTimer <= 0)
      {
        float listLobbiesTimerMax = 3f;
        listLobbiesTimer = listLobbiesTimerMax;
        LobbyList();
      }
    }
  }

  private void HandleHeartbeat()
  {
    if (IsLobbyHost())
    {
      heartBeatTimer -= Time.deltaTime;

      if (heartBeatTimer <= 0)
      {
        float heartBeatTImerMax = 15f;
        heartBeatTimer = heartBeatTImerMax;

        LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
      }
    }
  }

  private bool IsLobbyHost()
  {
    return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
  }

  public async void LobbyList()
  {
    try
    {
      QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
      {
        Filters = new List<QueryFilter>
        {
          new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
        }
      };

      QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
      OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
      {
        lobbyList = queryResponse.Results
      });
    } catch (LobbyServiceException e)
    {
      Debug.Log(e);
    }
  }

  public async void CreateLobby (string lobbyName, bool isPrivate)
  {
    OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);
    try
    {
      joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, KitchenGameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
    {
      IsPrivate = isPrivate
    });
      
      KitchenGameMultiplayer.Instance.StartHost();
      Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
      
    } catch (LobbyServiceException e)
    {
      OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
     Debug.Log(e);
    }
  }

  public async void QuickJoin()
  {
    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    try
    {
      joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
      KitchenGameMultiplayer.Instance.StartClient();
      
    } catch (LobbyServiceException e)
    {
      OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
      Debug.Log(e);
    }
  }

  public async void JoinWithCode(string lobbyCode)
  {
    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    try
    {
      joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
      KitchenGameMultiplayer.Instance.StartClient();
    } catch (LobbyServiceException e)
    {
      OnJoinFailed?.Invoke(this, EventArgs.Empty);
      Debug.Log(e);
    } 
  }
  
  public async void JoinWithId(string lobbyId)
  {
    OnJoinStarted?.Invoke(this, EventArgs.Empty);
    try
    {
      joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
      KitchenGameMultiplayer.Instance.StartClient();
    } catch (LobbyServiceException e)
    {
      OnJoinFailed?.Invoke(this, EventArgs.Empty);
      Debug.Log(e);
    } 
  }
  
  public Lobby GetLobby()
  {
    return joinedLobby;
  }

  public async void DeleteLobby()
  {
    if (joinedLobby != null)
    {
      try
      {
        await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        joinedLobby = null;
      } catch (LobbyServiceException e)
      {
        Debug.Log(e);
      }
    }
  }

  public async void LeaveLobby()
  {
    if (joinedLobby != null)
    {
      try
      {
        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
        joinedLobby = null;
      } catch (LobbyServiceException e)
      {
        Debug.Log(e);
      }
    }
  }

  public async void KickPlayer (string playerId)
  {
    if (IsLobbyHost())
    {
      try
      {
        await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
      } catch (LobbyServiceException e)
      {
        Debug.Log(e);
      }
    }
  }
}
