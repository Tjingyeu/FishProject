using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnListener : MonoBehaviour
{
    [HideInInspector] public ItemSpawner Spawner;
    private GameManager gameManager;
    private void Start()
    {
        Spawner = GetComponent<ItemSpawner>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
        // Subscribe to the event when the script is enabled
        Item.OnItemSpawned += HandleItemSpawned;

        EnemyAI.OnEnemySpawned += HandleEnemySpawn;
    }

    private void OnDestroy()
    {
        Item.OnItemSpawned -= HandleItemSpawned;
        EnemyAI.OnEnemySpawned -= HandleEnemySpawn;
    }

    private void HandleItemSpawned(Item item)
    {
        gameManager.SendMatchState(
            OpCodes.ItemInit,
            MatchDataJson.ItemInit(item.posIndex, item.index));    
    }

    private void HandleEnemySpawn(EnemyAI enemyAI)
    {
        gameManager.SendMatchState(
            OpCodes.EnemySpawned,
            MatchDataJson.EnemySpawn(enemyAI.posIndex, enemyAI.modelIndex, enemyAI.groupNumber));
    }
}
