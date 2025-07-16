using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

public class PlayerGunShoot : MonoBehaviour
{
    [SerializeField]
    GameObject bulletObject;
    [SerializeField]
    GameObject bulletParentObject;
    [SerializeField]
    float shootOffset = 1.0f;
    [SerializeField]
    LayerMask groundLayer;

    [SerializeField]
    float pressTimePerCharge = 0.5f;
    [SerializeField]
    int maxCharges = 1;

    // cursor manager
    bool shootPressed;
    bool shootBlocked;
    Vector3 shootDir;
    // total time
    float shootTimePerBullet;
    float cooldownTime;
    // timers
    float cooldownTimer;
    float pressTimer;

    // Animation
    Transform gunSprite;

    void Start()
    {
        shootPressed = false;
        shootBlocked = false;
        shootDir = Vector3.zero;
        // danger: called before bullet is instantiated, but since we won't change bullet's attribultes...
        Bullet bulletController = bulletObject.GetComponent<Bullet>();
        shootTimePerBullet = bulletController.GetShootTime();
        cooldownTimer = cooldownTime = pressTimer = 0;
        gunSprite = transform.GetChild(0);
    }

    void Update()
    {
        // Update cooldown
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer > cooldownTime || Global.state == STAGE_STATE.TIME_STOPPED)
        {
            cooldownTimer = cooldownTime;
        }

        // Update Shoot
        // Update shoot direction and check if blocked by tilemap
        UpdateShootDir();

        // Animation
        if (cooldownTimer < cooldownTime && !shootBlocked)
        {
            // for each bullet shoot, the gun moves from back (-0.2) to origin(0)
            float offset = -0.2f * (1 - (cooldownTimer / shootTimePerBullet) % 1);
            gunSprite.localPosition = new Vector3(offset, 0, 0);
        }
        else
        {
            gunSprite.localPosition = Vector3.zero;
        }

        if (Global.state == STAGE_STATE.TIME_STOPPED || Global.playerDead)
        {
            // time stopped or dead
            shootPressed = false;
            return;
        }

        if (shootPressed)
        {
            // charge
            pressTimer += Time.deltaTime;
            // release
            if (Input.GetMouseButtonUp(0))
            {
                // shoot
                if (!shootBlocked)
                {
                    // charge
                    // 0, 1, 2, 3
                    int charges = Mathf.Min((int)(pressTimer / pressTimePerCharge), maxCharges);
                    int bulletMaxLife = charges > 0 ? 7 : 1; // 1 5 7 10

                    GameObject bulletInstance = Instantiate(
                        bulletObject,
                        transform.position + shootDir * shootOffset,
                        Quaternion.FromToRotation(Vector3.right, shootDir),
                        bulletParentObject.transform
                    );
                    bulletInstance.GetComponent<Bullet>().maxLife = bulletMaxLife;
                    bulletInstance.tag = "PlayerBullet";
                    bulletInstance.layer = LayerMask.NameToLayer("PlayerBullet");

                    // cooldownTime = (charges + 1) * cooldownTimePerCharge;
                    cooldownTime = bulletMaxLife * shootTimePerBullet;
                    // Debug.Log("shootTime" + cooldownTime);
                }
                // reset timers
                cooldownTimer = 0;
                shootPressed = false;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // TODO: mask some noise
            if (cooldownTimer < cooldownTime)
            {
                // TODO: mask some noise

            }
            else
            {
                shootPressed = true;
                pressTimer = 0;
            }
        }
    }

    private void UpdateShootDir()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 shootTarget = Camera.main.ScreenToWorldPoint(mousePos);

        // dir
        shootDir = shootTarget - transform.position;
        shootDir.z = 0;
        shootDir.Normalize();

        // check if blocked
        // time stopped, no need to calculate
        if (Global.state == STAGE_STATE.TIME_STOPPED)
        {
            shootBlocked = true;
            return;
        }
        // if gun overlap with tilemap
        // since we use outline for tilemap collision, the bullet will be shoot into the walls
        // set shootBlocked to avoid this
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, shootDir, shootOffset, groundLayer);
        shootBlocked = hit ? true : false;
    }

    // for sprite and cursor control
    public float GetChargeRate()
    {
        if (!shootPressed)
        {
            return -1;
        }
        //float charge = pressTimer
        return Mathf.Clamp01(pressTimer / pressTimePerCharge);
        // return Mathf.Min(pressTimer / pressTimePerCharge, maxCharges);
    }

    public bool GetShootDisabled()
    {
        return Global.state == STAGE_STATE.TIME_STOPPED // time stopped
            || Global.playerDead
            || cooldownTimer < cooldownTime //  shooting
            || shootBlocked;
    }

    public bool GetShooting()
    {
        return cooldownTimer < cooldownTime;
    }
}
