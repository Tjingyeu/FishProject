using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BitingSystem : MonoBehaviour
{
    protected Rigidbody rb;
    protected LvlUpManager lvlDesign;
    protected Transform headTrans;

    private FixedJoint mouthJoint;
    private int itemScore;

    public void BitingStart(Collider other)
    {
        //grabbing the object with fixed joint
        mouthJoint = transform.gameObject.AddComponent<FixedJoint>();
        mouthJoint.connectedBody = other.attachedRigidbody;
        rb.isKinematic = true;


        //for gaining score and xp
        if (other.TryGetComponent(out Item item))
        {
            //destroy item and gain xp
            lvlDesign.XPGain(item.SetScore(lvlDesign.currentLvl));

            if (headTrans.CompareTag("Player"))
                item.isPlayer = true;
        }

        //bite damage
        IBitable bitedObject =
            other.TryGetComponent(out IBitable iBitable) ? iBitable : other.GetComponentInParent<IBitable>();
        bitedObject.BiteDmg();
    }

    public void BitingEnd()
    {
        //release the target when biting finishes
        //mouthJoint.connectedBody = null;
        Destroy(GetComponent<FixedJoint>());
        rb.isKinematic = false;
    }

    public IEnumerator Bite(Collider other)
    {
        BitingStart(other);

        yield return new WaitForSeconds(headTrans.GetComponent<PlayerSystem>().ATTACK_ACTION_TIME);

        BitingEnd();
    }

    protected void InitializeComponents()
    {
        headTrans = transform.parent.parent;
        lvlDesign = headTrans.GetComponent<LvlUpManager>();
        rb = headTrans.GetComponent<Rigidbody>();
    } 
}
