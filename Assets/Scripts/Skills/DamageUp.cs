using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageUp : SkillSystem
{
    protected override void Start()
    {
        base.Start();
        OnSkillEffect += DamageUpEffect;
        OnSkillEnded += DamageUpEffectEnded;
    }

    private void DamageUpEffect()
    {
        player.attack_dmg *= 2;
    }

    private void DamageUpEffectEnded()
    {
        player.attack_dmg /= 2;
    }
}
