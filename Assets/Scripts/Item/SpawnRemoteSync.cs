using UnityEngine;

public class SpawnRemoteSync : MonoBehaviour
{
    public GameObject enemy_prefab;
    public void ItemSpawn(int posIndex, int index)
    {
        Instantiate(DataManager.instance.itemPrefabs[index], DataManager.instance.spawnPoints[posIndex], Quaternion.identity);
    }

    public void MeatSpawn(Vector3 position, int index)
    {
        Instantiate(DataManager.instance.meats[index], position, Quaternion.identity);
    }

    public void RemoteSpawnAI(int posIndex, int modelIndex, int groupIndex)
    {
        EnemyAI enemyAI = Instantiate(enemy_prefab, DataManager.instance.spawnPoints[posIndex], Quaternion.identity).GetComponent<EnemyAI>();

        //enemyAI.AddComponent<AIRemoteSync>();

        enemyAI.InitializeDetails(modelIndex, groupIndex);

        enemyAI.GetComponentInChildren<HealthBarFade>().playerDied.AddListener((GameObject go) => { go.SetActive(false); Destroy(go); });
    }
}
