using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionExplode : MonoBehaviour
{
    // explode
    [SerializeField]
    GameObject explodeEffect;
    [SerializeField]
    GameObject explodeEffectForPlayer;
    [SerializeField]
    AudioClip explodeAudio;
    [SerializeField]
    AudioClip explodeAudioForPlayer;
    [SerializeField]
    Color explodeColor;

    int areaId;
    BoxCollider2D collider;
    EnemyActionDynamic enemyControl;
    SpriteRenderer spriteRenderer;

    void Start()
    {
        areaId = GetComponent<EnemyCommon>().GetAreaId();
        collider = GetComponent<BoxCollider2D>();
        enemyControl = GetComponent<EnemyActionDynamic>();
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        collider.isTrigger = Global.state == STAGE_STATE.NORMAL && Global.areaId == areaId;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collider.isTrigger)
        {
            // bullets are trigger so triggerEnter will be called
            // even if collider is not trigger
            return;
        }
        if (collision.CompareTag("Player"))
        {
            // TODO: set player hurt
            Explode(true);
        }
        else if (collision.CompareTag("PlayerBullet"))
        {
            Explode();
        }

    }
    private void Explode(bool forPlayer = false)
    {
        // dead
        // Destroy(gameObject);
        StartCoroutine(ExplodeCoroutine(forPlayer));
    }

    IEnumerator ExplodeCoroutine(bool forPlayer)
    {
        if (forPlayer)
        {
            spriteRenderer.color = explodeColor;
            yield return new WaitForSeconds(0.05f);
        }
        Instantiate(
            forPlayer ? explodeEffectForPlayer : explodeEffect,
            transform.position,
            transform.rotation
        );
        // play sound
        if (!Global.GetEndingTriggered())
        {
            Global.soundAudioSource.PlayOneShot(forPlayer ? explodeAudioForPlayer : explodeAudio);
        }
        spriteRenderer.color = Color.white;
        enemyControl.SetDead();
    }
}
