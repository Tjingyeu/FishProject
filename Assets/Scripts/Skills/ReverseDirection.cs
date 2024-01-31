using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReverseDirection : SkillSystem
{
    private RaycastHit hit;
    private Vector3 reverseDir;

    public float reverseSpeed = 20f;
    public float maxEffect = 20f;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        OnSkillProcess += ReverseEffect;
    }

    private void ReverseEffect()
    {
        if (Physics.SphereCast(transform.position, 2f, transform.forward, out hit, maxEffect))
        {
            reverseDir = (hit.transform.position - transform.position).normalized;
            hit.transform.position -= reverseSpeed * Time.deltaTime * reverseDir;
        }
    }
}
