using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTextController : MonoBehaviour
{
    int numActive;
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        numActive = -1;
    }

    void Update()
    {
        if (Global.state != STAGE_STATE.RESTART_COUNTDOWN || Global.areaState != AREA_STATE.ACTIVE)
        {
            // TODO: Hide
            if (numActive >= 0)
            {
                transform.GetChild(numActive).gameObject.SetActive(false);
            }
            return;
        }
        int num = Global.GetCountDownInteger();
        if (num != numActive)
        {
            if (numActive >= 0)
            {
                transform.GetChild(numActive).gameObject.SetActive(false);
            }
            transform.GetChild(num).gameObject.SetActive(true);
            numActive = num;
        }

    }
}
