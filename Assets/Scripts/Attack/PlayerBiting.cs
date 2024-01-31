using UnityEngine;

public class PlayerBiting : BitingSystem
{
    private void Start()
    {
        InitializeComponents();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
            if (!other.isTrigger)
            {
                StartCoroutine(Bite(other));
                //disable functioning after biting happens
                transform.GetChild(0).gameObject.SetActive(false);
            }
    }
}