using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public List<GameObject> playerModels = new();

    public List<Vector3> spawnPoints = new();

    public List<GameObject> itemPrefabs = new();

    public List<GameObject> meats = new();


    public static DataManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
    }
}