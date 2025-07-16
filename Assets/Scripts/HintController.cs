using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class HintController : MonoBehaviour
{
    [SerializeField]
    bool showOnTimeStopped = false;
    [SerializeField]
    bool showOnEnemiesDead = false;
    [SerializeField]
    int maxAreaId = 3;
    [SerializeField]
    float alphaSpeed = 1;
    [SerializeField]
    Transform[] enemiesCheckDeath;

    bool inside = false;
    bool fadeIn = false;
    bool fadeOut = false;
    float currAlpha = 0;
    SpriteRenderer hintRenderer;
    BoxCollider2D triggerCollider;

    void Start()
    {
        inside = fadeIn = fadeOut = false;
        currAlpha = 0;
        hintRenderer = GetComponent<SpriteRenderer>();
        triggerCollider = GetComponent<BoxCollider2D>();

        hintRenderer.color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        // update alpha
        if (CheckFadingIn())
        {
            UpdateFadeIn();
        }
        else if (CheckFadingOut())
        {
            UpdateFadeOut();
        }

        // max area
        UpdateMaxArea();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        inside = true;
        fadeIn = true;
        fadeOut = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            return;
        }
        inside = false;
        fadeIn = false;
        fadeOut = true;
    }



    private bool CheckFadingIn()
    {
        // on enter
        if (fadeIn && (!showOnTimeStopped || Global.state == STAGE_STATE.TIME_STOPPED) && !showOnEnemiesDead)
        {
            return true;
        }
        // on time stopped while inside
        if (inside && currAlpha < 1 && showOnTimeStopped && Global.state == STAGE_STATE.TIME_STOPPED )
        {
            return true;
        }
        if (inside && currAlpha < 1 && showOnEnemiesDead  && CheckEnemiesDead() )
        {
            return true;
        }
        return false;
    }

    private bool CheckFadingOut()
    {
        // exit trigger
        if (fadeOut)
        {
            return true;
        }
        // showOnTimeStopped, fade on no time not stopped
        return showOnTimeStopped && Global.state != STAGE_STATE.TIME_STOPPED && currAlpha > 0;
    }

    private bool CheckEnemiesDead()
    {
        foreach (Transform enemy in enemiesCheckDeath)
        {
            EnemyActionDynamic enemyActionDynamic = enemy.gameObject.GetComponent<EnemyActionDynamic>();
            if (enemyActionDynamic != null && !enemyActionDynamic.GetDead())
            {
                return false;
            }
        }
        Debug.Log("dead");
        return true;
    }

    private void UpdateFadeIn()
    {
        // update alpha
        currAlpha += alphaSpeed * Time.deltaTime;

        // done
        if (currAlpha >= 1)
        {
            currAlpha = 1;
            fadeIn = false;
        }

        hintRenderer.color = new Color(1, 1, 1, currAlpha);
    }

    private void UpdateFadeOut()
    {
        // update alpha
        currAlpha -= alphaSpeed * Time.deltaTime;

        // done
        if (currAlpha <= 0)
        {
            currAlpha = 0;
            fadeOut = false;
        }

        hintRenderer.color = new Color(1, 1, 1, currAlpha);
    }

    private void UpdateMaxArea()
    {
        if (Global.areaId <= maxAreaId)
        {
            return;
        }
        if (!triggerCollider.enabled)
        {
            // already disabled (TODO: this should be an event)
            return;
        }
        triggerCollider.enabled = false;
        fadeIn = false;
        fadeOut = false;
        hintRenderer.color = new Color(1, 1, 1, 0);
    }
}
