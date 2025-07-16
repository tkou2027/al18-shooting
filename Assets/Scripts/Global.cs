using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum STAGE_STATE
{
    NORMAL,
    TIME_STOPPED,
    RESTART_COUNTDOWN,
    END_COUNTDOWN
};
public enum AREA_STATE
{
    IDLE,
    ACTIVE,
    RESTARTING,
    CLEARED
};

public class Global : MonoBehaviour
{
    // 0 - normal
    // 1 - time stopped
    // 2 - restart countdown
    public static STAGE_STATE state;

    static readonly float timeRestartTime = 3.0f;
    static float timeRestartTimer = 0f;

    // player
    public static Transform player;
    public static bool playerDead = false;

    // area
    public static readonly int AREA_MAX = 8;
    public static int areaId = -1;
    public static AREA_STATE areaState = AREA_STATE.ACTIVE;
    public static Transform activeSavePoint;
    static bool areaResetDone = false;
    static readonly float areaRestartTime = 1f;
    static readonly float areaRestartTimeHalf = areaRestartTime / 2f;
    static float areaRestartTimer = 0f;

    // ending
    static bool endingTriggered = false;
    static readonly float endingTime = 5f;
    static float endingTimer = 0f;

    // area activate
    static bool areaActivating = false;
    static readonly float areaActivateTime = 0.5f;
    static float areaActivateTimer = 0f;

    // time trigger
    public static Transform activeTimeTrigger;

    // parent transform of all bullets
    public static Transform bulletsParent;

    // audio source
    public static AudioSource soundAudioSource;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bulletsParent = GameObject.Find("BulletsParent").transform;
        soundAudioSource = GameObject.FindGameObjectWithTag("SEAudio").GetComponent<AudioSource>();

        // init variables
        state = STAGE_STATE.NORMAL;
        playerDead = false;
        // time
        timeRestartTimer = 0f;

        // area
        areaId = -1;
        areaState = AREA_STATE.ACTIVE;
        areaResetDone = false;
        areaRestartTimer = 0f;

        // ending
        endingTriggered = false;
        endingTimer = 0f;

        // area activate
        areaActivating = false;
        areaActivateTimer = 0f;
    }
    private void Update()
    {
        UpdateStage();
        UpdateArea();
        UpdateEnding();
        UpdateAreaActivate();
    }

    private void UpdateStage()
    {
        // stage state
        if (state == STAGE_STATE.TIME_STOPPED)
        {
            if (Input.GetMouseButtonDown(1)) // right click
            {
                Debug.Log("Countdown");
                timeRestartTimer = 0;
                state = STAGE_STATE.RESTART_COUNTDOWN;
            }
        }
        else if (state == STAGE_STATE.RESTART_COUNTDOWN)
        {
            timeRestartTimer += Time.deltaTime;
            if (timeRestartTimer >= timeRestartTime)
            {
                Debug.Log("Die");
                timeRestartTimer = timeRestartTime;
                state = STAGE_STATE.NORMAL;
            }
        }
    }

    private void UpdateArea()
    {
        // Update Area State
        if (areaState == AREA_STATE.RESTARTING)
        {
            // Update restart
            //if (areaRestartTimer == 0)
            //{
            //    areaResetDone = false;
            //}
            // reset game objects at half of restart countdown
            if (areaRestartTimer >= areaRestartTimeHalf && !areaResetDone)
            {
                // reset visuals at half of restart countdown
                ResetArea();
            }
            // reset countdown end
            if (areaRestartTimer >= areaRestartTime)
            {
                // restart end
                areaState = AREA_STATE.ACTIVE;
            }
            // update timer
            areaRestartTimer += Time.deltaTime;
        }
        else
        {
            areaRestartTimer = 0;
            if (playerDead || Input.GetKeyDown(KeyCode.R))
            {
                // initialize area restart
                areaState = AREA_STATE.RESTARTING;
                areaResetDone = false;
                if (playerDead)
                {
                    areaRestartTimer = -0.5f;
                }
            }
        }
    }

    private void UpdateAreaActivate()
    {
        if (!areaActivating)
        {
            return;
        }
        areaActivateTimer += Time.deltaTime;
        if (areaActivateTimer >= areaActivateTime || areaState == AREA_STATE.RESTARTING)
        {
            areaActivateTimer = 0;
            areaActivating = false;
        }
    }

    private void ResetArea()
    {
        // normal state, no time stop / time countdown
        state = STAGE_STATE.NORMAL;
        // destroy all bullets 
        foreach (Transform child in bulletsParent)
        {
            Destroy(child.gameObject);
        }
        // player pos
        playerDead = false;
        if (activeSavePoint != null)
        {
            player.position = activeSavePoint.position + new Vector3(0, 1, 0);
        }
        // enemies will reset themselves

        // reset only once
        areaResetDone = true;
    }

    public static bool GetAreaResetDone()
    {
        return areaState == AREA_STATE.RESTARTING && areaResetDone;
    }

    public static float GetCountdownLerp()
    {
        // returns ratio of color in active state
        if (state == STAGE_STATE.NORMAL)
        {
            return 1f;
        }
        else if (state == STAGE_STATE.TIME_STOPPED)
        {
            return 0f;
        }
        // for bullet and enemy color flash
        float t = Mathf.Sin(timeRestartTimer * 4f * Mathf.PI) + 1f * 0.5f;
        return t;
    }

    public static int GetCountDownInteger()
    {
        return Mathf.FloorToInt(Mathf.Min(timeRestartTime, timeRestartTime - timeRestartTimer + 1));
    }

    public static float GetAreaRestartLerp()
    {
        // returns ratio of fade color
        if (areaState != AREA_STATE.RESTARTING)
        {
            return 0f;
        }
        // linear [-1, 1], 0 when timer == areaRestartTimeHalf
        float t = (Mathf.Max(areaRestartTimer, 0) / areaRestartTimeHalf) - 1;
        t = Mathf.Clamp(t, -1f, 1f); // avoid precision issues
        return 1 - t * t; // ease in/out, 1 when timer == areaRestartTimeHalf
    }

    public static float GetAreaActivateLerp()
    {
        if (!areaActivating)
        {
            return -1;
        }
        return areaActivateTimer / areaActivateTime;
    }

    // ending
    private void UpdateEnding()
    {
        if (!endingTriggered)
        {
            return;
        }
        endingTimer += Time.deltaTime;
        if (endingTimer >= endingTime)
        {
            endingTimer = endingTime;
            SceneManager.LoadScene("Ending");
        }
    }
    public static void TriggerEnding()
    {
        if (!endingTriggered)
        {
            endingTriggered = true;
            endingTimer = 0f;
        }
    }

    public static bool GetEndingTriggered()
    {
        return endingTriggered;
    }

    public static float GetEndingLerp()
    {
        // return ratio of ending fade
        if (!endingTriggered)
        {
            return 0f;
        }
        return endingTimer / endingTime;
    }

    public static void ActivateArea(int id, Transform savePoint)
    {
        areaId = id;
        areaState = AREA_STATE.ACTIVE;
        activeSavePoint = savePoint;
        areaActivating = true;
        areaActivateTimer = 0f;
    }
}
