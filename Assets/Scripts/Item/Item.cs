using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour,IBitable
{
    public delegate void ItemSpawnEventHandler(Item item);

    public static event ItemSpawnEventHandler OnItemSpawned;

    public GameObject bubble;
    public bool isPlayer = false;
    public int score;
    [HideInInspector]public int index;
    [HideInInspector]public int posIndex;
    public void SpawnItem()
    {
        OnItemSpawned?.Invoke(this);
    }

    private void OnDestroy()
    {
        if (!gameObject.scene.isLoaded) 
            return;
    }

    public void BiteDmg()
    {
        if(isPlayer)
            UIManager.instance.ShowText(transform.position, "+" + score, Color.yellow);

        if (TryGetComponent(out Animator animator))
            animator.enabled = true;

        GameObject bubbleIns = Instantiate(bubble, transform.position, Quaternion.identity);
        Destroy(gameObject, 0.8f);
        Destroy(bubbleIns, 0.8f);
    }

    public int SetScore(int itemPower)
    {
        score = Random.Range(5, 10) * itemPower;
        return score;
    }
}
