using UnityEngine;
using UnityEngine.UI;

public class FindMatch : MonoBehaviour
{
    public GameObject FindMatchBtn;
    public GameObject FindingBtn;
    public GameObject inputButton;
    public GameObject StartMenu;
    public GameObject DailyRewards;
    public GameObject loadingCircle;
    public Button FindMatchButton;
    public Button CancelMatchmakingButton;
    public Button DailyReward_btn;
    public Button close_dailyRWD_btn;

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
        FindMatchButton.onClick.AddListener(Find_Match);
        close_dailyRWD_btn.onClick.AddListener(DailyRewardMenuClose);
        DailyReward_btn.onClick.AddListener(DailyRewardMenu);
        CancelMatchmakingButton.onClick.AddListener(CancelMatchmaking);
        FindMatchButton.interactable = false;
    }

    #region 
    private void OnDestroy()
    {
        // Remove event listeners for the menu buttons.
        FindMatchButton.onClick.RemoveListener(Find_Match);
        CancelMatchmakingButton.onClick.RemoveListener(CancelMatchmaking);
    }

    public void EnableFindMatchButton()
    {
        FindMatchButton.interactable = true;
    }
    public void DisableFindMatchButton()
    {
        FindMatchButton.interactable = false;
    }

    public void DeactivateMenu()
    {
        FindMatchBtn.SetActive(false);
        FindingBtn.SetActive(false);
        StartMenu.SetActive(false);
        
        //gameObject.SetActive(false);
        inputButton.SetActive(true);
        loadingCircle.SetActive(false);
    }

    public void DailyRewardMenu()
    {
        DailyRewards.SetActive(true);
    }
    public void DailyRewardMenuClose()
    {
        DailyRewards.SetActive(false);
    }
    public async void Find_Match()
    {
    
        FindMatchBtn.SetActive(false);
        FindingBtn.SetActive(true);

        //PlayerPrefs.SetString("Name", NameField.text);
        //gameManager.SetDisplayName(NameField.text);
        Debug.Log("finding");
        loadingCircle.SetActive(true);

        await gameManager.WakaConnection.FindMatch(playerNum);

        Debug.Log("matches" + " with size " + playerNum);
    }


    public async void CancelMatchmaking()
    {
        FindMatchBtn.SetActive(true);
        FindingBtn.SetActive(false);
        loadingCircle.SetActive(false);

        await gameManager.WakaConnection.CancelMatchmaking();
    }
    #endregion
}
