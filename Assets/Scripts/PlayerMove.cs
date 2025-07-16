using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    float maxSpeed = 6.0f;

    [SerializeField]
    float maxAcc = 600.0f;
    [SerializeField]
    float maxAccAir = 5.0f;

    [SerializeField]
    float maxJumpHeight = 2.2f;
    [SerializeField]
    float maxJumpHeightBounce = 2.7f;

    [SerializeField]
    LayerMask groundLayer;
    [SerializeField]
    LayerMask bounceLayer;
    [SerializeField]
    float groundDepth = 1.2f;
    [SerializeField]
    float groundWidthHalf = 0.4f;
    [SerializeField]
    float minGroundNormal = 0.7f;

    Rigidbody2D body;

    // move input
    Vector2 desiredVelocity = Vector2.zero;
    bool desiredJump = false;

    // on ground
    [SerializeField]
    bool onGround = false;
    [SerializeField]
    bool onGroundBounce = false;
    Vector2 contactNormal = Vector2.zero;

    // Animatior
    Transform playerSprite;
    SpriteRenderer playerSpriteRenderer;
    Animator playerSpriteAnimator;

    // shoot status
    PlayerGunShoot shoot;

    void Start()
    {
        // Global.player = transform;
        body = GetComponent<Rigidbody2D>();
        playerSprite = transform.GetChild(0);
        playerSpriteRenderer = playerSprite.GetComponent<SpriteRenderer>();
        playerSpriteAnimator = playerSprite.GetComponent<Animator>();
        shoot = transform.GetChild(1).GetComponent<PlayerGunShoot>();

        desiredJump = false;
        onGround = false;
        onGroundBounce = false;
        contactNormal = Vector2.zero;
    }

    void Update()
    {
        // Usre Input
        desiredVelocity = new Vector2(Input.GetAxis("Horizontal"), 0f) * maxSpeed;
        desiredJump |= Input.GetButtonDown("Jump");

        // On Ground
        onGround = false;
        onGroundBounce = false;
        contactNormal = Vector3.zero;
        for (int i = -1; i <= 1; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position + new Vector3(i * groundWidthHalf, 0, 0),
                Vector2.down, groundDepth, groundLayer
            );
            if (hit)
            {
                // Debug.Log("hit" + hit.transform.name);
                onGround = true;
                contactNormal += hit.normal;
                int hitLayer = hit.transform.gameObject.layer;
                if (((1 << hitLayer) & bounceLayer) != 0)
                {
                    onGroundBounce = true;
                }
            }
        }
        contactNormal.Normalize();

        // animations
        playerSpriteAnimator.SetBool("isJumping", !onGround);
        playerSpriteAnimator.SetFloat("velocityY", body.velocityY);
        playerSpriteAnimator.SetFloat("velocityX", body.velocityX);

        if (Global.playerDead)
        {
            return;
        }
        // trun around
        Vector3 shootTarget = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 lookDir = shootTarget - transform.position;
        if (lookDir.x < 0)
        {
            playerSpriteRenderer.flipX = true;
        }
        else if (lookDir.x > 0)
        {
            playerSpriteRenderer.flipX = false;
        }

    }

    void FixedUpdate()
    {
        if (Global.areaState == AREA_STATE.RESTARTING || Global.playerDead || shoot.GetShooting())
        {
            desiredJump = false;
            body.velocity = Vector2.zero;
            return;
        }
        // Jump
        if (desiredJump)
        {
            desiredJump = false;
            if (onGround)
            {
                float maxHeight = onGroundBounce ? maxJumpHeightBounce : maxJumpHeight;
                float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * body.gravityScale * maxHeight);
                body.velocityY += jumpSpeed;
            }
        }
        // fall faster
        if (body.velocityY < 0 && !onGround)
        {
            body.velocityY += 0.5f * Physics.gravity.y * body.gravityScale * Time.deltaTime;
        }
        // TODO: project contact normal
        float maxSpeedChange = onGround ? maxAcc : maxAccAir * Time.deltaTime;
        // Debug.Log(Mathf.MoveTowards(body.velocityX, desiredVelocity.x, maxSpeedChange));
        float velocityX = Mathf.MoveTowards(body.velocityX, desiredVelocity.x, maxSpeedChange);
        body.velocityX = velocityX;
    }

    // debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = -1; i <= 1; i ++)
        {
            Vector3 positionOffset = transform.position + new Vector3(groundWidthHalf * i, 0, 0);
            Gizmos.DrawLine(positionOffset, positionOffset + Vector3.down * groundDepth);
        }
    }
}
