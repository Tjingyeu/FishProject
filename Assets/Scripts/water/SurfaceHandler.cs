using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceHandler : MonoBehaviour
{
    public GameObject WaterDrop;
    private void OnTriggerEnter(Collider other)
    {
        WaterExplode(other, 0.5f);

        if(other.TryGetComponent(out Item item))
        {

        }else
        {
            if(other.transform.parent.TryGetComponent(out PlayerSystem playerSystem))
            {
                playerSystem.jumped = true;
            }
        }
    }

    private void WaterExplode(Collider other, float duration)
    {
        Vector3 WaterDropSpawnPos = new(other.transform.position.x, transform.position.y, transform.position.z);
        GameObject waterinstance = Instantiate(WaterDrop, WaterDropSpawnPos, Quaternion.identity);

        Destroy(waterinstance, duration);
    }
}
