using UnityEngine;

public class EnemyAI : PlayerSystem
{
    public delegate void EnemyAIEventHandler(EnemyAI enemyAI);
    public static event EnemyAIEventHandler OnEnemySpawned;
    //patrol variables
    [SerializeField] Transform patrolStart;
    [SerializeField] Transform patrolEnd;
    private Vector3 patrolStartPos;
    private Vector3 patrolEndPos;
    ///

    private bool turnBack = false;
    private bool canBite = false;
    public enum State { Attack, Idle }
    public State currentState;
    public bool stateChanged;

    [HideInInspector] public int posIndex;

    // Start is called before the first frame update
    private void Start()
    {
        speed = 1500f;

        posIndex = Random.Range(0, DataManager.instance.spawnPoints.Count);

        ResetPatrolPosition();

        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        RayCastDetection();

        CheckState();

        switch (currentState)
        {
            case State.Attack:
                Attack();
                break;
            case State.Idle:
                Patroling();
                break;
        }
    }

    //we need to get the direction 
    private void Patroling()
    {
        if (transform.position.x <= patrolStartPos.x)
            turnBack = true;
        else if (transform.position.x >= patrolEndPos.x)
            turnBack = false;

        if (turnBack)
            TowardTarget(TargetDir(patrolEndPos), speed);
        else
            TowardTarget(TargetDir(patrolStartPos), speed);
    }

    //checking for the new position of patroling
    private void ResetPatrolPosition()
    {
        stateChanged = false;
        patrolStartPos = patrolStart.position;
        patrolEndPos = patrolEnd.position;
    }

    private void TowardTarget(Vector3 direction, float _speed)
    {
        if (!rb.isKinematic)
            rb.velocity = _speed * Time.deltaTime * direction;
        //for rotating the face of the model
        ModelTransform(direction);
    }

    private void Attack()
    {
        stateChanged = false;
        nearestTarget = GetNearestTarget();
        if (nearestTarget == null)
        {
            currentState = State.Idle;
            return;
        }

        TowardTarget(TargetDir(nearestTarget.position), 2 * speed);

        if (!canBite)
        {
            canBite = true;
            mouth.gameObject.SetActive(true);
            animator.SetTrigger("eat");
        }

        if (canBite)
        {
            timer += Time.deltaTime;
            if (timer >= ATTACK_ACTION_TIME)
            {
                mouth.gameObject.SetActive(false);
            }
            if (timer >= ATTACK_RATE + 0.2f)
            {
                timer = 0f;
                canBite = false;
            }
        }
    }

    private void CheckState()
    {
        if (target.Count == 0)
        {
            if(currentState != State.Idle)
            {
                stateChanged = true;
                currentState = State.Idle;
                ResetPatrolPosition();
            }
        }else
        {
            if(currentState != State.Attack)
            {
                stateChanged = true;
                currentState = State.Attack;
            }
        }
    }

    public void SpawnEnemy()
    {
        OnEnemySpawned?.Invoke(this);
    }
}