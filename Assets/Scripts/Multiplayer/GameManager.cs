using Nakama;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public WakawakaConnection WakaConnection;
    [SerializeField] private GameObject NetworkLocalPlayerPrefab;
    [SerializeField] private GameObject NetworkRemotePlayerPrefab;
    [SerializeField] private float TotalTime = 720;
    [SerializeField] private GameObject endGameMenu;
    [SerializeField] private GameObject startGameMenu;
    [SerializeField] private GameObject InputButtons;
    [HideInInspector] public bool timerstart =false;
    public static GameObject localPlayer;

    public enum GameMode
    {
        Team,Solo
    }
    public static GameMode gameMode;
    public int teamNumber;

    private float startingTime = 0;
    private IDictionary<string, GameObject> players;
    private IUserPresence localUser;
    private IMatch currentMatch;
    private string localDisplayName;

    private async void Start()
    {
        players = new Dictionary<string, GameObject>();

        await WakaConnection.Connect();
        startGameMenu.GetComponent<FindMatch>().EnableFindMatchButton();
        WakaConnection.Socket.ReceivedMatchmakerMatched += OnReceivedMatchmakerMatched;
        WakaConnection.Socket.ReceivedMatchPresence += OnReceivedMatchPresence;
        WakaConnection.Socket.ReceivedMatchState += m => OnReceivedMatchState(m);
    }
    private void Update()
    {
        StartTimer();
    }

    private async void OnReceivedMatchmakerMatched(IMatchmakerMatched matched)
    {

        Debug.Log("match is found");
        localUser = matched.Self.Presence;
        print(localUser + "aaaa this is localUser");
        var match = await WakaConnection.Socket.JoinMatchAsync(matched);
        Debug.Log(match.Presences.Count());
        startGameMenu.GetComponent<FindMatch>().DeactivateMenu();
        InputButtons.SetActive(true);
        foreach (var user in match.Presences)
        {
            SpawnPlayer(match.Id, user, -1);
            Debug.Log("from matched event" + user);
        }
        //CreateTeams();
        currentMatch = match;
        timerstart = true;
    }

    private void OnReceivedMatchPresence(IMatchPresenceEvent matchPresenceEvent)
    {
        
        foreach (var user in matchPresenceEvent.Joins)
        {
            Debug.Log("matchPresenceEvent"+user);
            
            SpawnPlayer(matchPresenceEvent.MatchId, user, -1);
        }


        foreach (var user in matchPresenceEvent.Leaves)
        {
            if (players.ContainsKey(user.SessionId))
            {
                Destroy(players[user.SessionId]);
                players.Remove(user.SessionId);
            }
        }
    }
    private void OnReceivedMatchState(IMatchState matchState)
    {
        var userSessionId = matchState.UserPresence.SessionId;
        var state = matchState.State.Length > 0 ? JsonConvert.DeserializeObject<IDictionary<string, string>>(Encoding.UTF8.GetString(matchState.State)) : null;
        switch (matchState.OpCode)
        {
            case OpCodes.Died:
                var playerToDestroy = players[userSessionId];
                Destroy(playerToDestroy, 0.5f);
                players.Remove(userSessionId);
                break;
            case OpCodes.Respawned:
                SpawnPlayer(currentMatch.Id, matchState.UserPresence, int.Parse(state["spawnIndex"]));
                break;
            default:
                break;

        }
    }
    private void SpawnPlayer(string matchId, IUserPresence user, int spawnIndex = -1)
    {
        // If the player has already been spawned, return early.
        if (players.ContainsKey(user.SessionId))
        {

            Debug.Log("return happened player containskey");
            return;
            
        }

        // If the spawnIndex is -1 then pick a spawn point at random, otherwise spawn the player at the specified spawn point.
        Vector3 spawnPoint = spawnIndex == -1 ?
            DataManager.instance.spawnPoints[Random.Range(0,DataManager.instance.spawnPoints.Count)] :
            DataManager.instance.spawnPoints[spawnIndex];



        // Set a variable to check if the player is the local player or not based on session ID.
        bool isLocal = user.SessionId == localUser.SessionId;

        // Choose the appropriate player prefab based on if it's the local player or not.
        var playerPrefab = isLocal ? NetworkLocalPlayerPrefab : NetworkRemotePlayerPrefab;

        // Spawn the new player.
        GameObject player = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);

        

        // Setup the appropriate network data values if this is a remote player.
        if (!isLocal)
        {
            player.GetComponent<PlayerNetworkRemoteSync>().NetworkData = new RemotePlayerNetworkData
            {
                MatchId = matchId,
                
                User = user
            };
        }

        // Add the player to the players array.
        players.Add(user.SessionId, player);

        // If this is our local player, add a listener for the PlayerDied event.
        if (isLocal)
        {
            localPlayer = player;
            SelectPlayerModel(player);
            player.GetComponentInChildren<HealthBarFade>().playerDied.AddListener(OnLocalPlayerDied);
        }
    }
    private async void OnLocalPlayerDied(GameObject player)
    {
        player.SetActive(false);
        Debug.Log("it's dead");
        // Send a network message telling everyone that we died.
        await SendMatchStateAsync(OpCodes.Died, MatchDataJson.Died(player.transform.position, Random.Range(0, 3)));


        // Remove ourself from the players array and destroy our GameObject after 0.5 seconds.
        players.Remove(localUser.SessionId);
        Destroy(player, 0.5f);
    }
    public void CleanUp()
    {
        // Reset the winner player text label.
        //WinningPlayerText.text = "";

        // Remove ourself from the players array and destroy our player.
        //players.Remove(localUser.SessionId);
        //Destroy(localPlayer);

        /*// Choose a new spawn position and spawn our local player.
        var spawnIndex = Random.Range(0, SpawnPoints.transform.childCount - 1);
        SpawnPlayer(currentMatch.Id, localUser, spawnIndex);

        // Tell everyone where we respawned.
        SendMatchState(OpCodes.Respawned, MatchDataJson.Respawned(spawnIndex));*/
        SceneManager.LoadScene("OnlineTest", LoadSceneMode.Single);
    }
    public async Task QuitMatch()
    {
        // Ask Nakama to leave the match.
        await WakaConnection.Socket.LeaveMatchAsync(currentMatch);

        // Reset the currentMatch and localUser variables.
        currentMatch = null;
        localUser = null;

        // Destroy all existing player GameObjects.
        foreach (var player in players.Values)
        {
            Destroy(player);
        }

        // Clear the players array.
        players.Clear();
    }
    public async Task SendMatchStateAsync(long opCode, string state)
    {
        await WakaConnection.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }
    public void SendMatchState(long opCode, string state)
    {
        WakaConnection.Socket.SendMatchStateAsync(currentMatch.Id, opCode, state);
    }
    public async Task SetDisplayName(string displayName)
    {

        await WakaConnection.Client.UpdateAccountAsync(WakaConnection.Session, null, displayName);

    }
    /*    public void SetDisplayName(string displayName)
        {
            // We could set this on our Nakama Client using the below code:
            // await NakamaConnection.Client.UpdateAccountAsync(NakamaConnection.Session, null, displayName);
            // However, since we're using Device Id authentication, when running 2 or more clients locally they would both display the same name when testing/debugging.
            // So in this instance we will just set a local variable instead.
            localDisplayName = displayName;
        }*/

    public void StartTimer()
    {
        if (timerstart)
        {
            startingTime += Time.deltaTime;
            if (startingTime >= TotalTime)
            {
                Debug.Log("time is up");
                GameOver();
            }
        }  
    }

    public void GameOver()
    {
        endGameMenu.SetActive(true); 
    }

    private async void SelectPlayerModel(GameObject player)
    {
        player.GetComponent<NewPlayer>().InitializeDetails(UIManager.instance.selectedPlayerIndex, GameManager.GetTeamNumber());

        await Task.Delay(500);
        await SendMatchStateAsync(OpCodes.PlayerModelIndex,
            MatchDataJson.PlayerModelIndex(UIManager.instance.selectedPlayerIndex, player.GetComponent<NewPlayer>().groupNumber));
    }

    public static int GetTeamNumber()
    {
        if (gameMode == GameMode.Team)
            return Random.Range(0, 100) % 2;
        else
            return Random.Range(0, 10000);
    }
}