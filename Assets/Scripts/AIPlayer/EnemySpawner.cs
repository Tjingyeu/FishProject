using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy_prefab;
    void Start()
    {
        SpawnAI();
    }

    public void SpawnAI()
    {
        int pos_Index = Random.Range(0, DataManager.instance.spawnPoints.Count);

        EnemyAI enemyAI = Instantiate(enemy_prefab, DataManager.instance.spawnPoints[pos_Index], Quaternion.identity).GetComponent<EnemyAI>();

        //enemyAI.AddComponent<PlayerNetworkLocalSync>();

        enemyAI.posIndex = pos_Index;

        enemyAI.modelIndex = Random.Range(0, DataManager.instance.playerSmallModels.Count);

        enemyAI.InitializeDetails(enemyAI.modelIndex, GameManager.GetTeamNumber());

        enemyAI.GetComponentInChildren<HealthBarFade>().playerDied.AddListener(DeathEvent);

        enemyAI.SpawnEnemy();
    }

    private void DeathEvent(GameObject go)
    {
        SpawnAI();
        go.SetActive(false);
        Destroy(go);
    }
}
