using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaController : MonoBehaviour
{
    [SerializeField]
    int id;
    //[SerializeField]
    //AudioClip triggerAudio;
    // 0-idle 1-active 2-restarting 3-cleared
    AREA_STATE state = AREA_STATE.IDLE;
    CircleCollider2D savePointCollider;
    Animator savePointSpriteAnimator;
    void Start()
    {
        savePointCollider = GetComponent<CircleCollider2D>();
        Transform savePointSprite = transform.GetChild(0);
        savePointSpriteAnimator = savePointSprite.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Global.areaId > id)
        {
            // entered next stage, cleared
            state = AREA_STATE.CLEARED;
            return;
        }
        if (state == AREA_STATE.ACTIVE)
        {
            // restart area
            if (Global.areaState == AREA_STATE.RESTARTING)
            {
                Debug.Log("Restarting");
                state = AREA_STATE.RESTARTING;
            }
        }
        else if (state == AREA_STATE.RESTARTING)
        {
            if (Global.areaState == AREA_STATE.ACTIVE)
            {
                state = AREA_STATE.ACTIVE;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // save point
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        // Debug.Log("Saved");
        savePointCollider.enabled = false; // disable after first trigger
        // update global area state
        state = AREA_STATE.ACTIVE;
        // Global.areaId = id;
        // Global.areaState = state;
        // Global.activeSavePoint = transform;
        //Global.soundAudioSource.PlayOneShot(triggerAudio);
        Global.ActivateArea(id, transform);
        // Global.savePoint = savePoint;

        // Animation
        savePointSpriteAnimator.SetBool("touched", true);
    }
}
