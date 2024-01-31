using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SkillSystem : MonoBehaviour
{
    public delegate void SkillEvent();
    public SkillEvent OnSkillEffect;
    public SkillEvent OnSkillEnded;
    public SkillEvent OnSkillProcess;
    public Sprite skillImg;
    public float duration;
    public float cooldown;

    protected Button skillBtn;
    protected Image skillImage;
    protected NewPlayer player;
    protected float timer;
    protected bool effecting;

    protected virtual void Start()
    {
        if(transform.parent.CompareTag("Player"))
        {
            player = GetComponentInParent<NewPlayer>();
            //initializing the skill button
            skillBtn = UIManager.instance.skillBtn;
            skillBtn.image.sprite = skillImg;
            skillBtn.onClick.AddListener(SkillClicked);

            //initializing background skill image
            skillImage = UIManager.instance.skillImage;
            player.skill.sprite = skillImg;
            skillImage.sprite = skillImg;
        }
    }

    private void Update()
    {
        if(effecting)
        {
            OnSkillProcess?.Invoke();
            timer += Time.deltaTime;
            if(timer > duration)
            {
                timer = 0f;
                effecting = false;
                //hide inactive skill in status bar
                player.skill.DOFade(0f, 0.2f);

                OnSkillEnded?.Invoke();
            }
        }
    }
    private void SkillClicked()
    {
        skillBtn.interactable = false;
        skillBtn.image.fillAmount = 0f;

        //showing the active skill on status bar
        player.skill.DOFade(1f, 0.2f);

        effecting = true;

        OnSkillEffect?.Invoke();

        skillBtn.image.DOFillAmount(1f, cooldown).OnComplete(() =>
        {
            skillBtn.interactable = true;
        });
    }
}
