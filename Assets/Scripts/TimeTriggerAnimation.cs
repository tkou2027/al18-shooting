using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTriggerAnimation : MonoBehaviour
{
    [SerializeField]
    Color colorBase;
    [SerializeField]
    Color colorStopped;

    const float secondsToDegrees = -6f;
    Transform secondsPivot;
    SpriteRenderer secondsRenderer;

    private void Start()
    {
        secondsPivot = transform.GetChild(0);
        secondsRenderer = secondsPivot.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // rotate
        if (Global.state != STAGE_STATE.TIME_STOPPED)
        {
            TimeSpan time = DateTime.Now.TimeOfDay;
            secondsPivot.localRotation =
                Quaternion.Euler(0f, 0f, secondsToDegrees * (int)time.TotalSeconds);
        }

        // color
        secondsRenderer.color = Global.state == STAGE_STATE.NORMAL ? colorBase : colorStopped;
    }
}
