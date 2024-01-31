﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarFade : MonoBehaviour {

    private const float DAMAGED_HEALTH_FADE_TIMER_MAX = .6f;

    private Image barImage;
    private Image damagedBarImage;
    private Color damagedColor;
    private GameObject parent;
    private float damagedHealthFadeTimer;

    [HideInInspector]public PlayerDiedEvent playerDied;
    [HideInInspector]public HealthSystem healthSystem;

    private void Awake()
    {
        playerDied ??= new PlayerDiedEvent();
        barImage = transform.Find("bar").GetComponent<Image>();
        damagedBarImage = transform.Find("damagedBar").GetComponent<Image>();
        parent = transform.parent.parent.gameObject;
        damagedColor = damagedBarImage.color;
        damagedColor.a = 0f;
        damagedBarImage.color = damagedColor;
    }

    private void Start() {
        healthSystem = new HealthSystem(100);
        SetHealth(healthSystem.GetHealthNormalized());
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealed += HealthSystem_OnHealed;
        
        //transform.Find("damageBtn").GetComponent<Button_UI>().ClickFunc = () => healthSystem.Damage(10);
        //transform.Find("healBtn").GetComponent<Button_UI>().ClickFunc = () => healthSystem.Heal(10);
    }

    private void Update() {
        if (damagedColor.a > 0) {
            damagedHealthFadeTimer -= Time.deltaTime;
            if (damagedHealthFadeTimer < 0) {
                float fadeAmount = 5f;
                damagedColor.a -= fadeAmount * Time.deltaTime;
                damagedBarImage.color = damagedColor;
            }
        }
    }

    private void HealthSystem_OnHealed(object sender, System.EventArgs e)
    {
        SetHealth(healthSystem.GetHealthNormalized());
    }

    private void HealthSystem_OnDamaged(object sender, System.EventArgs e) 
    {
        if (damagedColor.a <= 0) {
            // Damaged bar image is invisible
            damagedBarImage.fillAmount = barImage.fillAmount;
        }
        damagedColor.a = 1;
        damagedBarImage.color = damagedColor;
        damagedHealthFadeTimer = DAMAGED_HEALTH_FADE_TIMER_MAX;

        if (healthSystem.GetHealthNormalized() <= 0)
        {
            playerDied.Invoke(parent);

            //spawning fresh meat
            GameObject meat = Instantiate(DataManager.instance.meats[Random.Range(0, 3)], parent.transform.position, parent.transform.rotation);
            meat.GetComponent<Item>().score = parent.GetComponent<LvlUpManager>().currentLvl;
        }

        SetHealth(healthSystem.GetHealthNormalized());
    }

    private void SetHealth(float healthNormalized) {
        barImage.fillAmount = healthNormalized;
    }

    public void SetHealthAmount(float amount)
    {
        SetHealth(amount);
    }
}
