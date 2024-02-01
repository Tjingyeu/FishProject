using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindMatch : MonoBehaviour
{
    public GameObject PartyFindMatchBtn;
    public GameObject soloFindMatchBtn;
    public GameObject PartyFinding;
    public GameObject SoloFinding;
    public GameObject inputButton;
    public GameObject StartMenu;
    public GameObject DailyRewards;
    public GameObject loadingCircle;
    public GameObject leaderboard;
    public GameObject TopMenu;
    

    public Button PartyFindMatchButton;
    public Button soloFindMatchButton;
    public Button PartyCancelButton;
    public Button SoloCancelButton;
    public Button DailyReward_btn;
    public Button close_dailyRWD_btn;
    public Button leaderBoard_btn;
    public Button leaderBoard_Close;
    private GameManager gameManager;
    [SerializeField] private int playerNum = 2;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (PlayerPrefs.HasKey("Name"))
        {
            //NameField.text = PlayerPrefs.GetString("Name");
        }

        // Add event listeners for the menu buttons.
        //for finding match
        PartyFindMatchButton.onClick.AddListener(Party_Find_Match);
        soloFindMatchButton.onClick.AddListener(Solo_Find_Match);
        //other menues
        close_dailyRWD_btn.onClick.AddListener(DailyRewardMenuClose);
        DailyReward_btn.onClick.AddListener(DailyRewardMenu);
        //for canceling the match
        PartyCancelButton.onClick.AddListener(CancelPartyMatchmaking);
        SoloCancelButton.onClick.AddListener(CancelSoloMatchmaking);
        leaderBoard_Close.onClick.AddListener(Close_leaderboard);
        leaderBoard_btn.onClick.AddListener(LeaderboardMenu);
        DisableFindMatchButton();
    }

    #region 
    private void OnDestroy()
    {
        // Remove event listeners for the menu buttons.
        PartyFindMatchButton.onClick.RemoveListener(Party_Find_Match);
        PartyCancelButton.onClick.RemoveListener(CancelPartyMatchmaking);
    }

    public void EnableFindMatchButton()
    {
        PartyFindMatchButton.interactable = true;
        soloFindMatchButton.interactable = true;
    }
    public void DisableFindMatchButton()
    {
        PartyFindMatchButton.interactable = false;
        soloFindMatchButton.interactable = false;
    }

    public void DeactivateMenu()
    {
        PartyFindMatchBtn.SetActive(false);
        PartyFinding.SetActive(false);
        StartMenu.SetActive(false);
        TopMenu.SetActive(false);

        
        //gameObject.SetActive(false);
        inputButton.SetActive(true);
        loadingCircle.SetActive(false);

        Destroy(UIManager.instance.currentModel);
    }

    public void DailyRewardMenu()
    {
        DailyRewards.SetActive(true);
    }
    public void DailyRewardMenuClose()
    {
        DailyRewards.SetActive(false);
    }
    public void LeaderboardMenu()
    {
        leaderboard.SetActive(true);
    }
    public void Close_leaderboard()
    {
        leaderboard.SetActive(false);
    }
    public async void Party_Find_Match()
    {
        //PartyFindMatchBtn.SetActive(false);
        soloFindMatchButton.interactable = false;
        PartyFinding.SetActive(true);

        //PlayerPrefs.SetString("Name", NameField.text);
        //gameManager.SetDisplayName(NameField.text);
        Debug.Log("finding");
        loadingCircle.SetActive(true);

        GameManager.gameMode = GameManager.GameMode.Team;

        await gameManager.WakaConnection.FindMatch(playerNum, "party");

        Debug.Log("matches" + " with size " + playerNum);
    }

    public async void Solo_Find_Match()
    {
        //PartyFindMatchBtn.SetActive(false);
        PartyFindMatchButton.interactable = false;
        SoloFinding.SetActive(true);

        //PlayerPrefs.SetString("Name", NameField.text);
        //gameManager.SetDisplayName(NameField.text);
        Debug.Log("finding");
        loadingCircle.SetActive(true);

        GameManager.gameMode = GameManager.GameMode.Solo;

        await gameManager.WakaConnection.FindMatch(playerNum, "solo");

        Debug.Log("matches" + " with size " + playerNum);
    }


    public async void CancelPartyMatchmaking()
    {
        //PartyFindMatchBtn.SetActive(true);
        PartyFinding.SetActive(false);
        loadingCircle.SetActive(false);

        soloFindMatchButton.interactable = true;

        await gameManager.WakaConnection.CancelMatchmaking();
    }

    public async void CancelSoloMatchmaking()
    {
        //PartyFindMatchBtn.SetActive(true);
        SoloFinding.SetActive(false);
        loadingCircle.SetActive(false);

        PartyFindMatchButton.interactable = true;

        await gameManager.WakaConnection.CancelMatchmaking();
    }
    #endregion
}
