using UnityEngine;

public class PlayerNetworkLocalSync : MonoBehaviour
{
    public delegate void SyncEvent();
    public event SyncEvent OnSync;

    public float StateFrequency = 0.1f;

    private GameManager gameManager;
    private OfflinePlayerInput playerInput;
    private EnemyAI enemyAi;
    private Rigidbody playerRigidbody;
    private Transform playerTransform;
    private float stateSyncTimer;
    private HealthBarFade health;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerTransform = GetComponent<Transform>();
        health = GetComponentInChildren<HealthBarFade>();

        if (TryGetComponent(out playerInput))
            OnSync += PlayerInputSync;

        if (TryGetComponent(out enemyAi))
            OnSync += EnemyStateSync;
    }

    private void LateUpdate()
    {
        if (stateSyncTimer <= 0)
        {
            gameManager.SendMatchState(
                OpCodes.playerVelocityAndPosition,
                MatchDataJson.PlayerVelocityAndPosition(playerRigidbody.velocity, playerTransform.position));
            gameManager.SendMatchState(
                OpCodes.Health,
                MatchDataJson.Health(health.healthSystem.GetHealthNormalized()));
            stateSyncTimer = StateFrequency;
        }

        stateSyncTimer -= Time.deltaTime;

        OnSync?.Invoke();

    }

    private void PlayerInputSync()
    {
        if (!playerInput.InputChanged)
            return;

        gameManager.SendMatchState(
            OpCodes.Input,
            MatchDataJson.Input(
                playerInput.HorizontalInput, 
                playerInput.VerticalInput, 
                playerInput.Attack, 
                playerInput.SpeedUp, 
                playerInput.NormalSpeed));
    }

    private void EnemyStateSync()
    {
        if (!enemyAi.stateChanged)
            return;

        gameManager.SendMatchState(
            OpCodes.Input,
            MatchDataJson.Enemystate((int)enemyAi.currentState)
            );
    }
}