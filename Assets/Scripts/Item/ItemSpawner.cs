using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public float spawnInterval = 7f; // Time between spawns
    private Transform playerTransform;
    public Vector3 currentItemPosition;
    public int currentItemIndex;
    private float xRangeL;
    private float xRangeR;
    private float yRangeU;
    private float yRangeD;

    private void Start()
    {
        playerTransform = GetComponent<Transform>();
        // Start the spawning process
        InvokeRepeating("SpawnItem", 1f, spawnInterval);
    }

    private void SpawnItem()
    {
        //// Get the screen boundaries
        //float screenWidth = 140f;
        //float screenHeight = 100f;

        //xRangeL = playerTransform.position.x - screenWidth / 2;
        //xRangeR = playerTransform.position.x + screenWidth / 2;
        //yRangeD = playerTransform.position.y - screenHeight / 2;
        //yRangeU = playerTransform.position.y + screenHeight / 2;

        //// Generate a random position within the screen boundaries
        //Vector3 randomPosition = new(Random.Range(xRangeL, xRangeR), Random.Range(yRangeD, yRangeU), 0f);
        int spawnIndex = Random.Range(0, DataManager.instance.spawnPoints.Count);

        Vector3 randomPosition = DataManager.instance.spawnPoints[spawnIndex];

        int randomIdex = Random.Range(0, DataManager.instance.items.Count);
        // Choose a random item prefab from the array
        GameObject randomItemSpawned = DataManager.instance.items[randomIdex];

        // Spawn the chosen item at the random position
        Instantiate(randomItemSpawned, randomPosition, Quaternion.identity);

        Item itemScript = randomItemSpawned.GetComponent<Item>();
        itemScript.index = randomIdex;
        itemScript.posIndex = spawnIndex;
        itemScript.SpawnItem();
    }
}
