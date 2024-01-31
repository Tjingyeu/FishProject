using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedUp : SkillSystem
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        OnSkillEffect += AttackSpeedUpEffect;
        OnSkillEnded += AttackSpeedUpEnd;
    }

    private void AttackSpeedUpEffect()
    {
        player.ATTACK_ACTION_TIME /= 2f;
        player.ATTACK_RATE /= 2f;
        player.animator.SetFloat("attackSpeed", 2f);
    }

    private void AttackSpeedUpEnd()
    {
        player.ATTACK_ACTION_TIME *= 2f;
        player.ATTACK_RATE *= 2f;
        player.animator.SetFloat("attackSpeed", 1f);
    }
}
