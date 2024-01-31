using DG.Tweening;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class NewPlayer : PlayerSystem
{
    private float horizontal;
    private float vertical;
    private bool SpeedUp;

    //stamina script
    public Image staminaProgress;
    public Image xpProgress;
    public Image skill;
    [HideInInspector]public bool isRushing;

    void FixedUpdate()
    {
        if (model == null)
        {
            return;
        }
        HandleMovement();
        HandleStamina();
    }
    private void HandleMovement()
    {
        if (transform.position.y >= 410)
        {
            direction = JumpAndDiveDirection();
        }
        if (transform.position.y <= 400)
        {
            if (targeting)
            {
                if(nearestTarget != null)
                    direction = (nearestTarget.position - transform.position).normalized;
            }
            else
            {
                direction = new Vector3(horizontal, vertical, 0f).normalized;
            }
        }

        if (!rb.isKinematic)
            rb.velocity = speed * Time.deltaTime * direction;

        //just for preventing unintended rotation :)
        if (horizontal != 0 || vertical != 0 || targeting || transform.position.y >= 410)
            ModelTransform(direction);
    }
    public void HandleStamina()
    {
        if (staminaProgress != null)
            if (SpeedUp)
            {
                if (staminaProgress.fillAmount > 0)
                    staminaProgress.fillAmount -= Time.deltaTime;
                else
                    speed = 2000;
            }
            else
            {
                if (staminaProgress.fillAmount < 1)
                    staminaProgress.fillAmount += Time.deltaTime;
            }
    }


    public void DetectTarget(bool active)
    {
        targeting = active;

        if (active)
        {
            animator.SetTrigger("eat");
            RayCastDetection();//for finding targets
        }

        mouth.gameObject.SetActive(active);
    }

    public override void RayCastDetection()
    {
        base.RayCastDetection();

        //check for the null target
        if (target.Count == 0)
        {
            targeting = false;
            return;
        }
        nearestTarget = GetNearestTarget();
    }

    private Vector3 JumpAndDiveDirection()
    {
        if (jumped)
        {
            timer = 0f;
            jumped = false;
            jumpDirection = direction;
        }
        timer += Time.deltaTime;
        return new Vector3(jumpDirection.x, Mathf.Lerp(jumpDirection.y, -jumpDirection.y, timer), 0f);
    }

    #region
    public void SetHorizontalMovement(float value)
    {
        horizontal = value;
    }
    public void SetVerticalMovement(float value)
    {
        vertical = value;
    }
    public void PlayDeathAnimation()
    {
        //add death animation
        return;
    }
    public void BoostSpeed()
    {
        if (!isRushing)
        {
            staminaProgress.DOFade(1f, 0.5f);
            SpeedUp = true;
            speed = 5000f;
        }
    }
    public void NormalSpeed()
    {
        if(!isRushing)
        {
            staminaProgress.DOFade(0f, 0.5f);
            SpeedUp = false;
            speed = 2000f;
        }
    }
    #endregion
}
