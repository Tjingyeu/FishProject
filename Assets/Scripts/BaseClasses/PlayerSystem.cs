using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSystem : MonoBehaviour, IBitable
{
    //attack properties
    [HideInInspector]public float ATTACK_RATE = 1f;
    [HideInInspector]public float ATTACK_ACTION_TIME = 0.8f;
    [HideInInspector]public int attack_dmg;

    [HideInInspector]public Transform model;
    [HideInInspector]public bool jumped = false;
    [HideInInspector]public Animator animator;
    [HideInInspector]public int groupNumber;

    public float speed = 1500f;
    public Image healthImage;

    protected Transform nearestTarget;
    protected Vector3 jumpDirection;
    protected Vector3 direction;
    protected Transform mouth;
    protected Rigidbody rb;
    protected bool targeting = false;
    protected List<Collider> target = new();
    protected LvlUpManager lvl;
    protected float timer;

    //we need to get the direction 
    public Vector3 TargetDir(Vector3 position) => (position - transform.position).normalized;
    public void ModelTransform(Vector3 direction)
    {
        Vector3 looking = Vector3.RotateTowards(model.transform.forward, direction, 20f * Time.deltaTime, 0.1f);
        model.transform.rotation = Quaternion.LookRotation(looking, model.transform.up);

        //for controlling the z position of the player model
        model.transform.SetPositionAndRotation(new Vector3(model.transform.position.x, model.transform.position.y, 0f),
         Quaternion.Lerp(model.transform.rotation, new Quaternion(0.0f, model.transform.rotation.x, 0f, 0f), 0.3f));
    }

    public void InitializeDetails(int _modelIndex, int group_Num)
    {
        groupNumber = group_Num;

        model = Instantiate(DataManager.instance.playerModels[_modelIndex],transform).transform;
        mouth = model.Find("mouthPlace").Find("mouth");
        animator = model.GetComponent<Animator>();

        //changing the color of the healthbar based on friend an foe
        HealthColor(GameManager.localPlayer.GetComponent<PlayerSystem>().groupNumber == groupNumber);

        //in the initialization ignore collision
        IgnoreCollisionWithSelf();

        lvl = GetComponent<LvlUpManager>();
        rb = GetComponent<Rigidbody>();

        attack_dmg = 10;
    }

    public Transform GetNearestTarget()
    {
        float temp = (transform.position - target[0].transform.position).sqrMagnitude;
        Transform nearer = null;

        for (int i = 0; i < target.Count; i++)
        {
            if ((transform.position - target[i].transform.position).sqrMagnitude <= temp)
            {
                nearer = target[i].transform;
                temp = (transform.position - target[i].transform.position).sqrMagnitude;

                if (!nearer.CompareTag("food"))
                    if (nearer.transform.parent.TryGetComponent(out PlayerSystem playerSystem))
                    {
                        if (playerSystem.groupNumber == groupNumber)
                        {
                            //for ignoring friends
                            Debug.Log("it is friend");
                            nearer = null;
                            continue;
                        }
                    }
            }
        }
        return nearer;
    }
    public virtual void RayCastDetection()
    {
        target = Physics.OverlapSphere(transform.position, 5f * (2 + lvl.currentLvl)).ToList();
        _ = target.Remove(GetComponentInChildren<Collider>());
    }

    //ignore collision with self
    private void IgnoreCollisionWithSelf()
    {
        Collider c1 = mouth.GetComponent<Collider>();
        Collider c2 = model.GetComponent<Collider>();
        Physics.IgnoreCollision(c1, c2, true);

        //we need to disable it for using on trigger
        c1.gameObject.SetActive(false);
    }

    public void HealthColor(bool teammate)
    {
        if (teammate)
        {
            healthImage.color = Color.white;
        }
        else
        {
            healthImage.color = new Color(222f, 22f, 22f, 255f) / 255f;
        }
    }
    public void BiteDmg()
    {
        GetComponentInChildren<HealthBarFade>().healthSystem.Damage(attack_dmg);
    }
}