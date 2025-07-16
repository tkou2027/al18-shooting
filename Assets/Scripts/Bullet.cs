using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Object Variables ==============
    [SerializeField]
    float maxSpeed = 20;

    [SerializeField]
    GameObject bulletObject;

    [SerializeField]
    GameObject bulletDestroyEffect;

    [SerializeField]
    float bulletGap = 0.5f;

    [SerializeField]
    float bulletWidth = 0.3f;

    [SerializeField]
    LayerMask bulletRayLayers;

    [SerializeField]
    LayerMask continuousRayLayers;

    [SerializeField]
    public int maxLife = 5;

    [SerializeField]
    AudioClip hitAudio;

    [SerializeField]
    AudioClip hitAudioForTimer;

    // Graph Variables ==============
    public enum BULLET_STATE
    {
        NORMAL,
        TIME_STOPPED,
        COUNTDOWN
    };

    BULLET_STATE state = BULLET_STATE.NORMAL;
    // 0 - normal
    // 1 - stopped
    // 2 - start count down

    // bullet group
    int startIndex = 0;
    int endIndex = 0;
    float bulletWidthHalf = 0;
    float bulletDistance = 0;

    // components
    Rigidbody2D body;
    BoxCollider2D collider;

    public float GetShootTime()
    {
        // may be called before bullet is instantiated, so don't use bulletDistance
        return (bulletWidth + bulletGap) / maxSpeed;
    }

    void Start()
    {
        // components
        collider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();

        // state
        state = BULLET_STATE.NORMAL;
        // state 0 init
        body.velocity = transform.right * maxSpeed;
        // pre calculate
        bulletWidthHalf = bulletWidth / 2f;
        bulletDistance = bulletWidth + bulletGap;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == BULLET_STATE.NORMAL) // normal
        {
            // state change
            if (Global.state == STAGE_STATE.TIME_STOPPED)
            {
                // befor enter state - stopped
                body.velocity = Vector2.zero;
                collider.isTrigger = false;
                // TODO: change collider scale & offset
                collider.offset = new Vector2(
                    -bulletDistance * (endIndex + startIndex - 1) / 2f,
                    collider.offset.y
                );
                collider.size = new Vector2(
                    bulletDistance * (endIndex - startIndex - 1) + bulletWidth,
                    collider.size.y
                );

                state = BULLET_STATE.TIME_STOPPED;
                return;
            }

            // update instance
            int endTarget = maxLife;
            float maxDistance = (endTarget - 1) * bulletDistance + bulletWidthHalf;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position, -transform.right, maxDistance, bulletRayLayers);
            if (hit)
            {
                float hitDistance = hit.distance;
                endTarget = (int)((hitDistance - bulletWidthHalf) / bulletDistance) + 1;
                //Debug.Log("Hit");
                //Debug.Log(hitDistance);
            }
            //Debug.Log("Max");
            //Debug.Log(maxDistance);

            //if (endIndex < endTarget)
            //{
            //    Debug.Log("Creation Begin StartIndex: " + startIndex + " EndIndex: " + endIndex + " EndTarget: " + endTarget);
            //}

            for (int i = endIndex; i < endTarget; ++i)
            {
                Instantiate(
                    bulletObject,
                    transform.position - transform.right * i * bulletDistance,
                    transform.rotation,
                    transform
                );
                // Debug.Log(endIndex);
                endIndex = i + 1;
            }
        }
        else if (state == BULLET_STATE.TIME_STOPPED)
        {
            // state change
            if (Global.state == STAGE_STATE.RESTART_COUNTDOWN)
            {
                state = BULLET_STATE.COUNTDOWN;
                return;
            }
        }
        else if (state == BULLET_STATE.COUNTDOWN)
        {
            // state change
            // stopped again
            if (Global.state == STAGE_STATE.TIME_STOPPED)
            {
                state = BULLET_STATE.TIME_STOPPED;
                return;
            }

            // time to die
            if (Global.state == STAGE_STATE.NORMAL)
            {
                for (int i = startIndex; i < endIndex; ++i)
                {
                    Instantiate(
                        bulletDestroyEffect,
                        transform.position - i * transform.right * bulletDistance,
                        transform.rotation
                    );
                }
                Destroy(gameObject);
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleTrigger(collision);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        HandleTrigger(collision);
    }

    private void HandleTrigger(Collider2D collision)
    {
        // TODO: tag compare / layer cannot work because we need collision later for timestop
        if (gameObject.tag == "PlayerBullet" && collision.CompareTag("Player"))
        {
            return;
        }

        if (collision.CompareTag("TimeTrigger"))
        {
            HandleTriggerTimeTrigger(collision);

        }
        else
        {
            Global.soundAudioSource.PlayOneShot(hitAudio);
        }

        // Debug.Log("Trigger StartIndex: " + startIndex + " EndIndex: " + endIndex);

        // Destroy
        if (endIndex > startIndex)
        {
            Transform firstChild = transform.GetChild(0);
            Instantiate(
                bulletDestroyEffect,
                firstChild.position,
                transform.rotation
            );
            Destroy(firstChild.gameObject); // first bullet
        }
        ++startIndex;

        // Reset to next
        if (endIndex > startIndex || endIndex < maxLife)
        {
            // Debug.Log("baa");
            collider.offset = collider.offset - new Vector2(bulletDistance, 0); // transform.GetChild(1).position;
            if (endIndex < startIndex)
            {
                ++endIndex;
            }
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("Trigger End StartIndex: " + startIndex + " EndIndex: " + endIndex);

    }

    private void OnDrawGizmos()
    {
        // TODO: this is not working for distance?
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 2);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.up * 2);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, transform.position + transform.right * collider.offset.x);
    }

    private void HandleTriggerTimeTrigger(Collider2D collision)
    {
        Global.state = STAGE_STATE.TIME_STOPPED;
        // update active time trigger pos (for visual effects)
        Global.activeTimeTrigger = collision.transform;
        // TODO: play time trigger sound
        Global.soundAudioSource.PlayOneShot(hitAudioForTimer, 2);
    }
}
