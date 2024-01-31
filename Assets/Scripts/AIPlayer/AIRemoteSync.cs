using Nakama;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AIRemoteSync : MonoBehaviour
{
    public float LerpTime = 0.05f;

    private GameManager gameManager;
    private Rigidbody rb;
    private float lerpTimer;
    private Vector3 lerpFromPosition;
    private Vector3 lerpToPosition;
    private bool lerpPosition;
    private HealthBarFade healthBarFade;
    private EnemyAI enemyAi;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        rb = GetComponent<Rigidbody>();
        healthBarFade = GetComponentInChildren<HealthBarFade>();
        enemyAi = GetComponent<EnemyAI>();

        gameManager.WakaConnection.Socket.ReceivedMatchState += OnReceivedMatchState;
    }

    private void OnReceivedMatchState(IMatchState matchState)
    {
        switch(matchState.OpCode)
        {
            case OpCodes.playerVelocityAndPosition:
                UpdateVelocityAndPositionFromState(matchState.State);
                break;
            case OpCodes.Health:
                GetHealthValue(matchState.State);
                break;
            case OpCodes.Input:
                GetState(matchState.State);
                break;
        }
    }

    private void UpdateVelocityAndPositionFromState(byte[] state)
    {
        try
        {
            var stateDictionary = GetStateAsDictionary(state);

            if (!rb.isKinematic)
                rb.velocity = new Vector3(
                    float.Parse(stateDictionary["velocity.x"]), 
                    float.Parse(stateDictionary["velocity.y"]),
                    0);

            var position = new Vector3(
                float.Parse(stateDictionary["position.x"]),
                float.Parse(stateDictionary["position.y"]),
                0);

            lerpFromPosition = transform.position;
            lerpToPosition = position;
            lerpTimer = 0;
            lerpPosition = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to deserialize match state data: " + ex.Message);
        }
    }

    private void GetHealthValue(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);
        var healthValue = float.Parse(stateDictionary["playerHealth"]);
        healthBarFade.SetHealthAmount(healthValue);
    }

    private void GetState(byte[] state)
    {
        var stateDictionary = GetStateAsDictionary(state);
        enemyAi.currentState = (EnemyAI.State)int.Parse(stateDictionary["stateNum"]);
    }
    private IDictionary<string, string> GetStateAsDictionary(byte[] state)
    {
        return JsonConvert.DeserializeObject<IDictionary<string, string>>(Encoding.UTF8.GetString(state));
    }
    // Update is called once per frame
    void LateUpdate()
    {

        if (!lerpPosition)
        {
            return;
        }

        transform.position = Vector3.Lerp(lerpFromPosition, lerpToPosition, lerpTimer / LerpTime);
        lerpTimer += Time.deltaTime;

        if (lerpTimer >= LerpTime)
        {
            transform.position = lerpToPosition;
            lerpPosition = false;
        }
    }
}
