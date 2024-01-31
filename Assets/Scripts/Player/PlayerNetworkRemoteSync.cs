using Nakama;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerNetworkRemoteSync : MonoBehaviour
{
    public RemotePlayerNetworkData NetworkData;
    public float LerpTime = 0.05f;
    private GameManager gameManager;
    private NewPlayer playerScript;
    private Transform playerTransform;
    private float lerpTimer;
    private Vector3 lerpFromPosition;
    private Vector3 lerpToPosition;
    private bool lerpPosition;
    private HealthBarFade healthBarFade;
    private SpawnRemoteSync itemRemoteSync;
    private bool remoteIsAttacking = false;
    private float attackTimer = 0;
    private Rigidbody rb;

    private void Start()
    {
        // Cache a reference to the required components.
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        playerScript = GetComponent<NewPlayer>();
        playerTransform = GetComponent<Transform>();
        itemRemoteSync = GetComponent<SpawnRemoteSync>();
        healthBarFade = GetComponentInChildren<HealthBarFade>();
        rb = GetComponent<Rigidbody>();

        // Add an event listener to handle incoming match state data.
        gameManager.WakaConnection.Socket.ReceivedMatchState += OnReceivedMatchState;
    }


    private void LateUpdate()
    {
        if (!lerpPosition)
        {
            return;
        }

        playerTransform.position = Vector3.Lerp(lerpFromPosition, lerpToPosition, lerpTimer / LerpTime);
        lerpTimer += Time.deltaTime;

        if (lerpTimer >= LerpTime)
        {
            playerTransform.position = lerpToPosition;
            lerpPosition = false;
        }

        if(remoteIsAttacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= playerScript.ATTACK_ACTION_TIME)
                playerScript.DetectTarget(false);
            if(attackTimer >= playerScript.ATTACK_RATE)
            {
                attackTimer = 0f;
                remoteIsAttacking = false;
            }
        }
    }

    private void OnDestroy()
    {
        if(gameManager != null)
        {
            gameManager.WakaConnection.Socket.ReceivedMatchState -= OnReceivedMatchState;
        }
    }


    private void OnReceivedMatchState(IMatchState matchState)
    {
        if(matchState.UserPresence.SessionId != NetworkData.User.SessionId)
        {
            return;
        }

        switch(matchState.OpCode)
        {
            case OpCodes.playerVelocityAndPosition:
                UpdateVelocityAndPositionFromState(matchState.State);
                break;
            case OpCodes.Input:
                SetInputFromState(matchState.State);
                break;
            case OpCodes.Died:
                playerScript.PlayDeathAnimation();
                SetMeatSpawnValue(matchState.State);
                break;
            case OpCodes.Health:
                GetHealthValue(matchState.State);
                break;
            case OpCodes.ItemInit:
                SetItemSpawnValue(matchState.State);
                break;
            case OpCodes.PlayerModelIndex:
                SetPlayerModelIndex(matchState.State);
                break;
            case OpCodes.EnemySpawned:
                SetEnemySpawn(matchState.State);
                break;
            default:
                break;
        }
    }

    private IDictionary<string, string> GetStateAsDictionary(byte[] state)
    {
        return JsonConvert.DeserializeObject<IDictionary<string, string>>(Encoding.UTF8.GetString(state));
    }

    private void GetHealthValue(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);
        var healthValue = float.Parse(stateDictionary["playerHealth"]);
        healthBarFade.SetHealthAmount(healthValue);
    }
    private void UpdateVelocityAndPositionFromState(byte[] state)
    {
        try
        {
            var stateDictionary = GetStateAsDictionary(state);

            if(!rb.isKinematic && rb != null)
                rb.velocity = new Vector3(float.Parse(stateDictionary["velocity.x"]), float.Parse(stateDictionary["velocity.y"]), 0);

            var position = new Vector3(
                float.Parse(stateDictionary["position.x"]),
                float.Parse(stateDictionary["position.y"]),
                0);

            lerpFromPosition = playerTransform.position;
            lerpToPosition = position;
            lerpTimer = 0;
            lerpPosition = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to deserialize match state data: " + ex.Message);
        }
    }

    private void SetInputFromState(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);

        playerScript.SetHorizontalMovement(float.Parse(stateDictionary["horizontalInput"]));
        playerScript.SetVerticalMovement(float.Parse(stateDictionary["verticalInput"]));

        if (bool.Parse(stateDictionary["speedUp"]))
        {
            playerScript.BoostSpeed();
        }
        if (bool.Parse(stateDictionary["normalSpeed"])) 
        {
            playerScript.NormalSpeed();
        }
        if(!remoteIsAttacking)
            if (bool.Parse(stateDictionary["attack"]))
            {
                remoteIsAttacking = true;
                playerScript.DetectTarget(true);
            }
    }
    private void SetItemSpawnValue(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);
        int posIndex = int.Parse(stateDictionary["posIndex"]);
        int itemIndex = int.Parse(stateDictionary["itemIndex"]);
        itemRemoteSync.ItemSpawn(posIndex, itemIndex);
    }
    private void SetPlayerModelIndex(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);
        int remoteModelIndex = int.Parse(stateDictionary["index"]);
        int remoteGroupNum = int.Parse(stateDictionary["groupNum"]);
        playerScript.InitializeDetails(remoteModelIndex, remoteGroupNum);
    }

    private void SetEnemySpawn(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);
        int enemyModelIndex = int.Parse(stateDictionary["modelIndex"]);
        int posIndex = int.Parse(stateDictionary["posIndex"]);
        int groupNum = int.Parse(stateDictionary["groupNum"]);

        itemRemoteSync.RemoteSpawnAI(posIndex, enemyModelIndex, groupNum);
    }
    private void SetMeatSpawnValue(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);
        int meatIndex = int.Parse(stateDictionary["meatIndex"]);
        Vector3 meatPos = new(float.Parse(stateDictionary["position.x"]),float.Parse(stateDictionary["position.y"]),0f);
        itemRemoteSync.MeatSpawn(meatPos, meatIndex);
    }
}
