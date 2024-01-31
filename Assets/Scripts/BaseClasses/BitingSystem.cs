using System;
using System.Collections;
using UnityEngine;

public class BitingSystem : MonoBehaviour
{
    protected Rigidbody rb;
    protected LvlUpManager lvlDesign;
    protected Transform headTrans;

    private FixedJoint mouthJoint;

    public void BitingStart(Collider other)
    {
        //grabbing the object with fixed joint
        mouthJoint = transform.gameObject.AddComponent<FixedJoint>();
        mouthJoint.connectedBody = other.attachedRigidbody;
        rb.isKinematic = true;

        //bite damage
        if (other.TryGetComponent(out IBitable bitedObject))
            bitedObject.BiteDmg(headTrans);
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
        if (other == null)
            yield return null;

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
