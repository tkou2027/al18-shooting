using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownSoundController : MonoBehaviour
{
    AudioSource countdownAudio;
    void Start()
    {
        countdownAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Global.state == STAGE_STATE.TIME_STOPPED || Global.areaState == AREA_STATE.RESTARTING)
        {
            countdownAudio.Stop();
            return;
        }
        if (Global.state == STAGE_STATE.RESTART_COUNTDOWN && !countdownAudio.isPlaying)
        {
            countdownAudio.Play();
        }
    }
}
