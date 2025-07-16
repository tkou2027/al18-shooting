using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHurt : MonoBehaviour
{
    // animation
    Animator playerSpriteAnimator;

    void Start()
    {
        Transform playerSprite = transform.GetChild(0);
        playerSpriteAnimator = playerSprite.GetComponent<Animator>();
    }
    void Update()
    {
        // Animation
        playerSpriteAnimator.SetBool("isHurt", Global.playerDead);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("EnemyBullet") || collider.CompareTag("EnemyDynamic"))
        {
            // TODO: this should be an event
            Global.playerDead = true;
        }
    }
}
