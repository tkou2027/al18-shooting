using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionShoot : MonoBehaviour
{
    [SerializeField]
    GameObject bulletObject;
    [SerializeField]
    int bulletLife = 7;
    [SerializeField]
    LayerMask triggerLayer;
    [SerializeField]
    float cooldownTime = 2f;
    [SerializeField]
    float rayLength = 20f;
    [SerializeField]
    float rayHalfWidth = 0.3f;
    [SerializeField]
    LayerMask groundLayer;

    int areaId;
    float cooldownTimer;
    Vector3 aimDir;
    Transform laserSprite;

    // Start is called before the first frame update
    void Start()
    {
        areaId = GetComponent<EnemyCommon>().GetAreaId();
        cooldownTimer = cooldownTime;
        aimDir = transform.right * transform.localScale.x; // static
        // init laser sprite
        laserSprite = transform.GetChild(2);
        laserSprite.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

        // update cooldown
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= cooldownTime)
        {
            cooldownTimer = cooldownTime;
        }

        UpdateLaserSprite();

        // if active
        if (Global.state != STAGE_STATE.NORMAL || Global.areaId != areaId)
        {
            return;
        }
        if (cooldownTimer >= cooldownTime)
        {
            for (int i = -1; i <= 1; i += 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position + transform.up * rayHalfWidth * i, aimDir, laserSprite.localScale.x, triggerLayer);
                if (hit)
                {
                    //GameObject hitObject = hit.collider.gameObject;
                    //if (hitObject.CompareTag("Player")
                    //|| hitObject.CompareTag("PlayerBullet")) { }
                    InitShoot();
                }

            }
        }
    }

    private void UpdateLaserSprite()
    {
        if (Global.state != STAGE_STATE.NORMAL || areaId != Global.areaId || cooldownTimer < cooldownTime)
        {
            laserSprite.localScale = Vector3.zero;
            return;
        }
        float laserLength = rayLength;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, aimDir, rayLength, groundLayer);
        if (hit)
        {
            laserLength = hit.distance;
        }
        laserLength = Mathf.Min(laserLength, laserSprite.localScale.x + 500.0f * Time.deltaTime);
        laserSprite.localScale = new Vector3(Mathf.Min(laserLength, laserLength), rayHalfWidth * 2, 1);
        laserSprite.localPosition = new Vector3(laserLength / 2, 0, 0);
    }

    private void InitShoot()
    {
        // shoot
        Vector3 shootDir = aimDir;
        shootDir.z = 0;
        shootDir.Normalize();

        GameObject bulletInstance = Instantiate(
            bulletObject,
            transform.position + shootDir * 0.3f,
            Quaternion.FromToRotation(Vector3.right, shootDir),
            Global.bulletsParent
        );
        bulletInstance.tag = "EnemyBullet";
        bulletInstance.GetComponent<Bullet>().maxLife = bulletLife;
        bulletInstance.tag = "EnemyBullet";
        bulletInstance.layer = LayerMask.NameToLayer("EnemyBullet");

        cooldownTimer = 0;
    }

    // debug
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = -1; i <= 1; i += 2)
        {
            Vector3 verticalOffset = transform.up * rayHalfWidth * i;
            Gizmos.DrawLine(transform.position + verticalOffset, transform.position + verticalOffset + transform.right * transform.localScale.x * rayLength);
        }
    }
}
