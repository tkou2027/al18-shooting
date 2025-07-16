using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionDynamic : MonoBehaviour
{
    [SerializeField]
    float speed = 2f;

    [SerializeField]
    float offsetX = 0f; // per instance offset
    // basic move
    [SerializeField]
    float minDeltaX = -1f;
    [SerializeField]
    float maxDeltaX = 1f;
    [SerializeField]
    int initialDirection = -1;

    // fall
    [SerializeField]
    float targetDeltaY = 0f;
    [SerializeField]
    float triggerYDeltaX = 1f;
    [SerializeField]
    float minDeltaXFall = 0f;
    [SerializeField]
    float maxDeltaXFall = 0f;
    [SerializeField]
    float gravityFactor = 1f;

    // Graphs Variables ====
    int areaId;
    ENEMY_STATE state;
    // 0-idle 1-active 2-stopped 3-dead 4-restarting

    // move
    int direction;
    Vector2 desiredVelocity;
    Vector2 beforeStopVelocity;
    Vector3 initialPos;
    bool falling;
    bool fallEnded;

    Rigidbody2D body;
    BoxCollider2D collider;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        areaId = GetComponent<EnemyCommon>().GetAreaId();
        state = ENEMY_STATE.IDLE;

        initialPos = transform.position;
        body = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        collider.isTrigger = Global.state == STAGE_STATE.NORMAL && Global.areaId == areaId;

        // Freeze
        // either area is not active or stage is restarting
        if (Global.areaId != areaId || Global.areaState == AREA_STATE.RESTARTING)
        {
            // AnyState to Freeze
            state = ENEMY_STATE.IDLE;

            // update falling even if area cleared
            if (
                falling
                && Global.areaId > areaId // area cleared
                && Global.state == STAGE_STATE.NORMAL && Global.areaState == AREA_STATE.ACTIVE // not time-stop or restart
                )
            {
                UpdateFalling();
            }
            else
            {
                // otherwise should not move
                desiredVelocity = Vector2.zero;
                // reset visuals during area restart countdown
                if (Global.areaId == areaId && Global.GetAreaResetDone())
                {
                    ResetVisual();
                }
            }
            return;
        }

        // Init
        if (state == ENEMY_STATE.IDLE && Global.areaId == areaId && Global.areaState == AREA_STATE.ACTIVE)
        {
            // restart
            EnterActive();
            return;
        }

        if (state == ENEMY_STATE.ACTIVE)
        {
            if (Global.state != STAGE_STATE.NORMAL)
            {
                // TODO: sprite
                beforeStopVelocity = body.velocity;
                desiredVelocity = Vector2.zero;
                state = ENEMY_STATE.STOPPED;
                // fix y into floor
                FixFallEnd();
                return;
            }

            UpdateMove();
        }
        else if (state == ENEMY_STATE.STOPPED)
        {
            if (Global.state == STAGE_STATE.NORMAL)
            {
                desiredVelocity = beforeStopVelocity;
                // desiredVelocity.y = 0;
                state = ENEMY_STATE.ACTIVE;
                return;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        body.velocity = desiredVelocity;
    }

    private void ResetVisual()
    {
        transform.position = initialPos;
        spriteRenderer.flipX = initialDirection > 0;
        spriteRenderer.enabled = true;
    }

    private void EnterActive()
    {
        // reset
        ResetVisual();
        collider.enabled = true;
        // init move
        body.velocityX = initialDirection * speed;
        direction = initialDirection;
        falling = false;
        fallEnded = false;
        state = ENEMY_STATE.ACTIVE;
    }


    private void UpdateMove()
    {
        float deltaX = transform.position.x - initialPos.x;

        if (!falling)
        {
            // update direction if not falling
            float currMinDeltaX = minDeltaX + offsetX + (fallEnded ? minDeltaXFall : 0);
            float currMaxDeltaX = maxDeltaX + offsetX + (fallEnded ? maxDeltaXFall : 0);

            if (deltaX <= currMinDeltaX)
            {
                direction = 1;
            }
            if (deltaX >= currMaxDeltaX)
            {
                direction = -1;
            }
        }
        // Set sprite
        spriteRenderer.flipX = direction > 0;
        desiredVelocity.x = direction * speed;

        if (targetDeltaY < 0 && !fallEnded)
        {
            // test if falling
            if (!falling)
            {
                if (CheckEnterFalling())
                {
                    falling = true;
                }
            }
            // update fall
            if (falling)
            {
                UpdateFalling();
            }
            else
            {
                // not falling
                desiredVelocity.y = 0;
            }
        }


        // desiredVelocity = new Vector2(direction * speed, desiredVelocityY);
    }

    private bool CheckEnterFalling()
    {
        if (falling || fallEnded)
        {
            // already falling / landed
            return false;
        }
        float appliedTriggerYDeltaX = triggerYDeltaX + offsetX;
        // fall when heading towards triggerYDeltaX
        if (direction * appliedTriggerYDeltaX < 0)
        {
            return false;
        }

        float deltaX = transform.position.x - initialPos.x;
        bool fallRight = direction > 0 && deltaX >= appliedTriggerYDeltaX;
        bool fallLeft = direction < 0 && deltaX <= appliedTriggerYDeltaX;
        return fallRight || fallLeft;
    }

    private bool CheckEnterFallEnd()
    {
        return transform.position.y <= initialPos.y + targetDeltaY + 0.02f;
    }

    private bool FixFallEnd()
    {
        if (CheckEnterFallEnd())
        {
            // set position
            Vector3 pos = transform.position;
            pos.y = initialPos.y + targetDeltaY;
            transform.position = pos;
            // set vy
            desiredVelocity.y = 0;
            // set flags
            falling = false;
            fallEnded = true;
            return true;
        }
        return false;
    }

    private void UpdateFalling()
    {
        if (!falling)
        {
            return;
        }
        // check fall in inactive stage
        if (Global.areaId != areaId)
        {
            desiredVelocity.x = 0;
        }

        // check x
        float deltaX = transform.position.x - initialPos.x;
        float currMinDeltaX = minDeltaX + offsetX + minDeltaXFall;
        float currMaxDeltaX = maxDeltaX + offsetX + maxDeltaXFall;
        if (deltaX <= currMinDeltaX || deltaX >= currMaxDeltaX)
        {
            desiredVelocity.x = 0;
        }

        // check fall end
        if (FixFallEnd())
        {
            return;
        }

        desiredVelocity.y += Physics2D.gravity.y * Time.deltaTime * gravityFactor;
    }

    public void SetDead()
    {
        desiredVelocity = Vector2.zero;
        spriteRenderer.enabled = false;
        collider.enabled = false;
        state = ENEMY_STATE.DEAD;
    }

    public bool GetDead()
    {
        return state == ENEMY_STATE.DEAD;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 left = transform.position + new Vector3(minDeltaX + offsetX, 0, 0);
        Vector3 right = transform.position + new Vector3(maxDeltaX + offsetX, 0, 0);
        Gizmos.DrawLine(left + new Vector3(0, -1, 0), left + new Vector3(0, 1, 0));
        Gizmos.DrawLine(right + new Vector3(0, -1, 0), right + new Vector3(0, 1, 0));

        // fall
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position + new Vector3(triggerYDeltaX + offsetX, 0, 0),
            transform.position + new Vector3(triggerYDeltaX + offsetX, targetDeltaY, 0));

        Gizmos.color = Color.blue;
        Vector3 fallLeft = transform.position + new Vector3(minDeltaX + offsetX + minDeltaXFall, targetDeltaY, 0);
        Vector3 fallRight = transform.position + new Vector3(maxDeltaX + offsetX + maxDeltaXFall, targetDeltaY, 0);
        Gizmos.DrawLine(fallLeft + new Vector3(0, -1, 0), fallLeft + new Vector3(0, 1, 0));
        Gizmos.DrawLine(fallRight + new Vector3(0, -1, 0), fallRight + new Vector3(0, 1, 0));
    }
}
