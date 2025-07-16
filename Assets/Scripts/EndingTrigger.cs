using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    [SerializeField]
    float endingDelay = 10.0f;
    [SerializeField]
    float endingTriggerX = -5.0f;

    BoxCollider2D triggerCollider;
    bool endingTriggered = false;
    float endingCountdown;
    private void Start()
    {
        triggerCollider = GetComponent<BoxCollider2D>();
        triggerCollider.enabled = false;
        endingTriggered = false;
        endingCountdown = 0f;
    }
    private void Update()
    {
        // TODO: this should be an event
        triggerCollider.enabled = Global.areaId == Global.AREA_MAX;
        
        // countdown, in case player never gets out
        if (endingTriggered)
        {
            endingCountdown += Time.deltaTime;
            if (endingCountdown >= endingDelay || Global.player.transform.position.x > endingTriggerX)
            {
                endingCountdown = endingDelay;
                Global.TriggerEnding();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        // TODO better timer
        if (!endingTriggered)
        {
            endingTriggered = true;
            endingCountdown = 0f;
        }
        // Global.TriggerEnding();
    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (!collision.CompareTag("Player"))
    //    {
    //        return;
    //    }
    //    Debug.Log("Exit Ending x=" + collision.transform.position.x + " y=" + collision.transform.position.y);
    //    Global.TriggerEnding();
    //}
}
