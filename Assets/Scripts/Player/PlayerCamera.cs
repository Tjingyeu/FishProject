using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 cameraCorrection;
    private float limitedX, limitedY;
    private Vector3 a = Vector3.zero;
    private Vector3 offset;

    // Start is called before the first frame update
    private void Start()
    {
        playerTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        cameraCorrection.x = playerTransform.position.x;
        cameraCorrection.y = playerTransform.position.y;
        cameraCorrection.z = playerTransform.position.z - 100;
        Camera.main.transform.position = cameraCorrection;
        //Camera.main.transform.position = Vector3.SmoothDamp(cameraCorrection, cameraCorrection, ref a, 0.2f);
        Clamping();
    }
    private void Clamping()
    {
        limitedY = Mathf.Clamp(transform.position.y, -45f, 500f);
        limitedX = Mathf.Clamp(transform.position.x, -400f, 400f);
        transform.position = new Vector3(limitedX, limitedY, transform.position.z);
    }
}
