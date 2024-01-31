using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Transform startPanel;
    [SerializeField] Text scoreText;

    [HideInInspector] public int selectedPlayerIndex = 0;
    [HideInInspector] public bool showText = false;
    [HideInInspector] public GameObject currentModel;

    public Button skillBtn;
    public Image skillImage;
    public Transform modelHolder;


    public static UIManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    private void Start()
    {
        currentModel = modelHolder.GetChild(0).gameObject;
    }
    //for selecting the player in menu
    public void SetPlayer(int modelIndex)
    {
        if (modelIndex != selectedPlayerIndex)
        {
            selectedPlayerIndex = modelIndex;
            Destroy(currentModel);
            currentModel = Instantiate(DataManager.instance.playerModels[modelIndex], modelHolder);
        }
    }

    public void ShowText(Vector3 pos,string score,Color color)
    {
        scoreText.transform.position = pos;
        scoreText.DOText(score, 0.3f);
        scoreText.DOColor(color, 0.8f);
        scoreText.transform.DOPunchScale(transform.localScale + new Vector3(0.2f, 0.2f, 0.2f), 0.8f);
        scoreText.transform.DOMoveY(transform.position.y + 10f, 0.8f).OnComplete(()=>
        {
            scoreText.DOFade(0f, 0.3f);
            scoreText.transform.localScale = Vector3.one;
            scoreText.text = "";
        });
    }
}
