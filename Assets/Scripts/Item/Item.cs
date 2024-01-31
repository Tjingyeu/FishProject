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

    public void BiteDmg(Transform other)
    {
        score = Random.Range(5, 10) * other.GetComponent<PlayerSystem>().lvl.currentLvl;


        if(GameManager.localPlayer != null)
            if (GameManager.localPlayer.transform == other)
                UIManager.instance.ShowText(transform.position, "+" + score, Color.yellow);

        if (TryGetComponent(out Animator animator))
            animator.enabled = true;


        Destroy(gameObject, 0.8f);
        //explode with bubble
        Destroy(Instantiate(bubble, transform.position, Quaternion.identity), 0.8f);

        //Destroy(this);//just diabling the script after bite

        other.GetComponent<PlayerSystem>().lvl.XPGain(score);
    }
}
