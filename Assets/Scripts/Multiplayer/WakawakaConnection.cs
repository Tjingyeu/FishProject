using Nakama;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WakawakaConnection : MonoBehaviour
{
    public string Scheme = "http";
    [SerializeField] private string Host = "43.143.243.201";
    [SerializeField] private int Port = 7350;
    public string ServerKey = "defaultkey";

    private const string SessionPrefName = "nakama.session";
    private const string DeviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";

    public IClient Client;
    public ISession Session;
    public ISocket Socket;
    public IApiAccount Account;
    public IApiUser User;
    public ISession SingletonSession { get; private set; }

    private string currentMatchmakingTicket;
    private string currentMatchId;

  

    public async Task Connect()
    {
        Client = new Client(Scheme,Host,Port,ServerKey,UnityWebRequestAdapter.Instance);

        var authToken = PlayerPrefs.GetString(SessionPrefName);
        if (!string.IsNullOrEmpty(authToken))
        {
            var session = Nakama.Session.Restore(authToken);
            if (!session.IsExpired)
            {
                Session = session;
            }
        }
        if (Session == null)
        {
            string deviceId;

            if(PlayerPrefs.HasKey(DeviceIdentifierPrefName))
            {
                deviceId = PlayerPrefs.GetString(DeviceIdentifierPrefName);
            }
            else
            {
                deviceId = SystemInfo.deviceUniqueIdentifier;
                if(deviceId == SystemInfo.unsupportedIdentifier)
                {
                    deviceId = System.Guid.NewGuid().ToString();
                }
                PlayerPrefs.SetString(DeviceIdentifierPrefName, deviceId);
            }
            Session = await Client.AuthenticateDeviceAsync(deviceId);
            PlayerPrefs.SetString(SessionPrefName, Session.AuthToken);
        }
        SingletonSession = Session;
        Account = await Client.GetAccountAsync(Session);
        User = Account.User;
        Socket = Client.NewSocket(true);
        await Socket.ConnectAsync(Session, true);

        Debug.Log(Session);
        Debug.Log(Socket);

    }

    public async Task FindMatch(int minPlayers, string gameMode)
    {
        Dictionary<string, string> stringProperties;
        string query;

        if (gameMode == "solo")
        {
            stringProperties = new Dictionary<string, string>() {
            {"gameMode", "solo"}};

            query = "+properties.gameMode:solo";
        }
        else
        {
            stringProperties = new Dictionary<string, string>() {
            {"gameMode", "party"}};

            query = "+properties.gameMode:party";
        }


        var matchmakerTicket = await Socket.AddMatchmakerAsync(query, minPlayers, minPlayers, stringProperties, null);
        currentMatchmakingTicket = matchmakerTicket.Ticket;

        print(matchmakerTicket);
    }

    public async Task CancelMatchmaking()
    {
        await Socket.RemoveMatchmakerAsync(currentMatchmakingTicket);
    }

}
