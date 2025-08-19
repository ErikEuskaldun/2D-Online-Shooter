using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Singleton;

    private Lobby joinedLobby;
    private float heartbeatTimer = HEARTBEAT_MAX_TIME;
    private float lobbyUpdateTimer = LOBBY_UPDATE_MAX_TIME;
    private bool inGame = false;

    [SerializeField] private string team = "red";
    [SerializeField] private GameObject networkManager;

    public Lobby JoinedLobby => joinedLobby;

    private const float HEARTBEAT_MAX_TIME = 15f;
    private const float LOBBY_UPDATE_MAX_TIME = 1.5f;

    //Events
    public event EventHandler<List<Lobby>> OnLobbyListUpdates;
    public event EventHandler<Lobby> OnLobbyJoined;
    public event EventHandler OnLobbyLeft;
    public event EventHandler<Lobby> OnLobbyDataChange;
    public event EventHandler OnGameStarting;

    private void Awake()
    {
        /*if (Singleton != null && Singleton !=this)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);*/
        NetworkManager[] existingManagers = FindObjectsByType<NetworkManager>(FindObjectsSortMode.None);
        if (existingManagers.Length == 0)
            Instantiate(networkManager);

        //DontDestroyOnLoad(gameObject);

        Singleton = this;
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyPollForUpdates();
    }

    //Mantiene el lobby activo
    private async void HandleLobbyHeartbeat()
    {
        if(joinedLobby != null)
        {
            heartbeatTimer -= Time.deltaTime;
            if(heartbeatTimer < 0f)
            {
                heartbeatTimer = HEARTBEAT_MAX_TIME;
                if(IAmHost())
                    await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    //Obtiene actualizaciones constantes del lobby
    private async void HandleLobbyPollForUpdates()
    {
        if (joinedLobby != null)
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0f)
            {
                lobbyUpdateTimer = LOBBY_UPDATE_MAX_TIME;
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);
                joinedLobby = lobby;
                if (joinedLobby.Data["relay_key"].Value != "0")
                    JoinGame();
                OnLobbyDataChange?.Invoke(this, joinedLobby);
            }
        }
    }

    //Crea un lobby con los datos ofrecidos
    public async void CreateLobby(string lobbyName = "DefaultLobby", int maxPlayers = 4, bool isPrivate = false)
    {
        try
        {
            //Opciones del lobby
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = isPrivate, //Se puede hacer privado para que solo se pueda unir por código o ID
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
                {
                    { "game_mode", new DataObject(DataObject.VisibilityOptions.Public, "Free-for-All") },
                    //{"GameMode", new DataObject(DataObject.VisibilityOptions.Public, "GunGame", DataObject.IndexOptions.S1) } #Para luego poder filtrar por S1
                    { "map", new DataObject(DataObject.VisibilityOptions.Public, "test_map") },
                    { "relay_key", new DataObject(DataObject.VisibilityOptions.Member, "0") },
                    { "build_version", new DataObject(DataObject.VisibilityOptions.Public, Application.version) },

                }
            };

            //Creacion del lobby
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, createLobbyOptions);
            //TODO: OnLobbyCreated
            OnLobbyJoined?.Invoke(this, joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Lista los lobbys que cumplan el filtro
    public async void ListLobbies()
    {
        try
        {
            //Filtro
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Count = 25,
                Filters = new List<QueryFilter> { 
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, 0.ToString(), QueryFilter.OpOptions.GT),
                    //new QueryFilter(QueryFilter.FieldOptions.S1, "GunGame", QueryFilter.OpOptions.EQ) #Buscar por modo
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            //Busqueda
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListUpdates?.Invoke(this, queryResponse.Results);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Se une a un lobby por ID
    public async void JoinLobbyByID(string id)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, joinLobbyByIdOptions);
            joinedLobby = lobby;
            OnLobbyJoined?.Invoke(this, joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Se une a un lobby por código
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            Debug.Log(lobbyCode);
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = GetPlayer()                
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions);
            joinedLobby = lobby;
            OnLobbyJoined?.Invoke(this, joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Se une al primer lobby que encuentra  disponible
    public async void QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions quickJoinLobbyOptions = new QuickJoinLobbyOptions
            {
                Player = GetPlayer()
            };
            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(quickJoinLobbyOptions);
            joinedLobby = lobby;
            OnLobbyJoined?.Invoke(this, joinedLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Obtiene la lista de los jugadores en el lobby actual
    private List<Player> GetPlayers()
    {
        return joinedLobby.Players;
    }

    //Información del jugador
    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
                {
                    { "username", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerDataManager.Instance.Username) },
                    { "level", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerDataManager.Instance.Level.ToString()) },
                    { "team", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, team) },
                }
        };
    }

    //Actualiza el modo de juego del lobby
    private async void UpdateLobbyGamemode(string gameMode = "game_mode")
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "game_mode", new DataObject(DataObject.VisibilityOptions.Public, gameMode) },
                }
            });
            joinedLobby = lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Actualiza el equipo del jugador
    private async void UpdatePlayerTeam(string team = "blue")
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    { "team", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, team) },
                }
            });
            joinedLobby = lobby;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Se sale del lobby actual
    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            OnLobbyLeft?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Expulsa a un jugador por indice
    private async void KickPlayer(int playerIndex)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, joinedLobby.Players[playerIndex].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    //Migra el host al indice del jugador
    private async void MigrateLobbyHost(int playerIndex)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                HostId = joinedLobby.Players[playerIndex].Id,
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Elimina el lobby
    private async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            OnLobbyLeft?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    //Inicia la partida con relay
    public async void StartGame()
    {
        if (!IAmHost())
            return;

        try
        {
            string relayCode = await RelayManager.Singleton.CreateRelay();

            Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    { "relay_key", new DataObject(DataObject.VisibilityOptions.Member, relayCode) }
                }
            });

            joinedLobby = lobby;

            OnGameStarting?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void JoinGame()
    {
        if (IAmHost() || inGame)
            return;

        inGame = true;

        RelayManager.Singleton.JoinRelay(joinedLobby.Data["relay_key"].Value);

        OnGameStarting?.Invoke(this, EventArgs.Empty);

    }

    //Comprueba si el lobby pertenece al usuario
    public bool IAmHost()
    {
        if (joinedLobby.HostId == AuthenticationService.Instance.PlayerId)
            return true;
        return false;
    }
}
