using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OfflinePlayerInput : MonoBehaviour
{
    [HideInInspector] public float HorizontalInput;
    [HideInInspector] public float VerticalInput;
    [HideInInspector] public bool SpeedUp;
    [HideInInspector] public bool NormalSpeed;
    [HideInInspector] public bool Attack;
    [HideInInspector] public bool InputChanged;
    [HideInInspector] public FloatingJoystick joystick;


    private bool speedUp;
    private bool normalSpeed;
    private bool attack = false;
    private float horizontalInput;
    private float verticalInput;
    private NewPlayer playerScript;
    private float timer;
    private bool isAttacking = false;


    public enum Device
    {
        windows,
        android
    }

    public Device device;

    private void Start()
    {
        timer = 0f;
        playerScript = GetComponent<NewPlayer>();
        joystick = GameObject.FindGameObjectWithTag("joystick").GetComponent<FloatingJoystick>();
    }

    private void Update()
    {
        if (device == Device.windows)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
            speedUp = Input.GetKeyDown(KeyCode.LeftShift);
            normalSpeed = Input.GetKeyUp(KeyCode.LeftShift);
            attack = Input.GetKeyDown(KeyCode.Space);
        }
        if (device == Device.android)
        {
            horizontalInput = joystick.Horizontal;
            verticalInput = joystick.Vertical;
        }



        InputChanged = (horizontalInput != HorizontalInput || verticalInput != VerticalInput || attack != Attack || speedUp != SpeedUp);
        HorizontalInput = horizontalInput;
        VerticalInput = verticalInput;
        Attack = attack;
        SpeedUp = speedUp;
        NormalSpeed = normalSpeed;



        playerScript.SetHorizontalMovement(HorizontalInput);
        playerScript.SetVerticalMovement(VerticalInput);


        if (isAttacking)
        {
            timer += Time.deltaTime;

            if (timer >= playerScript.ATTACK_ACTION_TIME)
            {
                playerScript.DetectTarget(false);
            }
            if(timer >= playerScript.ATTACK_RATE)
            {
                timer = 0f;
                isAttacking = false;
            }
        }
        else
        {
            if (attack)
            {
                isAttacking = true;
                attack = false;
                playerScript.DetectTarget(true);
            }
        }
    }

    #region handling Touch
    public void bitingAction(InputAction.CallbackContext context)
    {
        if (context.performed)
            attack = true;
    }
    public void boost(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            playerScript.BoostSpeed();
        }
    }
    public void normal(InputAction.CallbackContext context)
    {
        if (context.performed)
            playerScript.NormalSpeed();
    }
    #endregion
}
