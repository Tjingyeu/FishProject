using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    public Button reward;
    public Sprite[] array;
    public Image buttonImage;

    public void rewardClaimed()
    {
        reward.GetComponent<Image>().sprite = array[1];

    }
}
