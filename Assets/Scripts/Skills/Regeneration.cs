using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regeneration : SkillSystem
{
    private HealthSystem hs;
    private float regenTimer;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        hs = transform.parent.GetComponentInChildren<HealthBarFade>().healthSystem;

        OnSkillProcess += RegenEffectProcess;
    }

    private void RegenEffectProcess()
    {
        regenTimer += Time.deltaTime;
        if(regenTimer >= 1f)
        {
            regenTimer = 0f;
            hs.Heal(hs.healthAmountMax / 20);//healing 5 percent everySecond
        }
    }
}
