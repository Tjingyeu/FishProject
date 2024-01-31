using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LvlUpManager : MonoBehaviour
{
    public int currentLvl = 1;
    public int currentXP = 0;
    public int xpNeededToLvlUp = 100;

    public Text lvlText;
    public Image plusImg;

    private HealthBarFade health;
    private PlayerSystem playerSystem;
    private NewPlayer localPlayer;
    void Start()
    {
        lvlText.text = currentLvl.ToString();
        health = GetComponentInChildren<HealthBarFade>();
        playerSystem = GetComponent<PlayerSystem>();
    }


    public void XPGain(int amount)
    {
        currentXP += amount;

        if (currentXP >= xpNeededToLvlUp)
        {
            int extraXP = currentXP - xpNeededToLvlUp;
            currentXP = extraXP;
            LvlUp();
        }

        if (TryGetComponent(out localPlayer))
            localPlayer.xpProgress.fillAmount = (float)currentXP / xpNeededToLvlUp;
    }

    private void LvlUp()
    {
        currentLvl++;
        xpNeededToLvlUp += xpNeededToLvlUp / currentLvl;
        lvlText.text = currentLvl.ToString();

        UpgradePlayerStatus();

        LvlUpAction();
    }

    private void UpgradePlayerStatus()
    {
        playerSystem.attack_dmg += currentLvl * 5;
        health.healthSystem.healthAmountMax += 20 * currentLvl;
        health.healthSystem.healthAmount += 20 * currentLvl;
        health.SetHealthAmount(health.healthSystem.GetHealthNormalized());

        if(currentLvl == 2)
        {
            Destroy(playerSystem.model.gameObject);
            playerSystem.ModelInitialization(true);
            return;
        }
        playerSystem.model.DOScale(playerSystem.model.transform.localScale + new Vector3(0.3f, 0.3f, 0.3f), 1f);
    }

    private void LvlUpAction()
    {
        Vector3 originalPos = plusImg.transform.position;
        plusImg.DOFade(1f, 0.5f);
        plusImg.transform.DOPunchScale(transform.localScale + new Vector3(0.2f, 0.2f, 0.2f), 0.8f);
        plusImg.transform.DOMoveY(transform.position.y + 10f, 1f).OnComplete(() =>
        {
            plusImg.DOFade(0f, 0.5f);
            plusImg.transform.position = originalPos;
        });
    }
}
