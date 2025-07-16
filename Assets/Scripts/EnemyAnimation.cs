using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    int areaId;
    Transform sprite;
    Transform spriteFreeze;
    SpriteRenderer spriteRenderer;
    SpriteRenderer spriteRendererFreeze;
    // Start is called before the first frame update
    void Start()
    {
        areaId = GetComponent<EnemyCommon>().GetAreaId();

        sprite = transform.GetChild(0);
        spriteFreeze = transform.GetChild(1);
        spriteRenderer = sprite.GetComponent<SpriteRenderer>();
        spriteRendererFreeze = spriteFreeze.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        spriteRendererFreeze.flipX = spriteRenderer.flipX;
        spriteRendererFreeze.enabled = spriteRenderer.enabled;

        // idle
        if (Global.state == STAGE_STATE.TIME_STOPPED)
        {
            spriteRendererFreeze.color = Color.white;
        }
        else if (areaId != Global.areaId)
        {
            if (areaId > 0 && areaId == Global.areaId - 1)
            {
                float t = Global.GetAreaActivateLerp();
                if (t > 0)
                {
                    spriteRendererFreeze.color = new Color(1, 1, 1, t);
                    return;
                }
            }
            spriteRendererFreeze.color = Color.white;
        }
        else if (Global.state == STAGE_STATE.RESTART_COUNTDOWN)
        {
            float t = Global.GetCountdownLerp();
            spriteRendererFreeze.color = new Color(1, 1, 1, 1 - t);
        }
        else
        {
            float t = Global.GetAreaActivateLerp();
            if (t > 0)
            {
                spriteRendererFreeze.color = new Color(1, 1, 1, 1 - t);
                return;
            }
            spriteRendererFreeze.color = new Color(1, 1, 1, 0);
        }
    }
}
