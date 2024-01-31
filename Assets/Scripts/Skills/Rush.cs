using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rush : SkillSystem
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        OnSkillEffect += RushEffect;

        OnSkillEnded += RushEffectEnd;
    }

    private void RushEffect()
    {
        player.isRushing = true;
        player.speed = 5000f;
    }

    private void RushEffectEnd()
    {
        player.isRushing = false;
        player.speed = 2000f;
    }
}
