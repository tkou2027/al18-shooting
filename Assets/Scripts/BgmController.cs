using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmController : MonoBehaviour
{
    [SerializeField]
    float baseVolume = 0.3f;
    [SerializeField]
    float timeStoppedVolume = 0.3f;
    [SerializeField]
    float basePitch = 1.0f;
    [SerializeField]
    float timeStoppedPitch = 0.95f;

    AudioSource bgmAudioSource;
    private void Start()
    {
        bgmAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float volume = Global.state == STAGE_STATE.TIME_STOPPED ? timeStoppedVolume : baseVolume;
        bgmAudioSource.volume = volume * (1f - Global.GetEndingLerp());
        // bgmAudioSource.pitch = Global.state == STAGE_STATE.TIME_STOPPED ? timeStoppedPitch : basePitch;
    }
}
