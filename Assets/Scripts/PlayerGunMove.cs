using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunMove : MonoBehaviour
{
    [SerializeField]
    int maxAngleAcc = 360;

    Transform gunSprite;
    SpriteRenderer gunSpriteRenderer;
    PlayerGunShoot shoot;

    private void Start()
    {
        gunSprite = transform.GetChild(0);
        gunSpriteRenderer = gunSprite.GetComponent<SpriteRenderer>();
        shoot = GetComponent<PlayerGunShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shoot.GetShooting() || Global.playerDead)
        {
            // don't move while shooting or dead
            return;
        }
        // Rotate ====
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetDir = mousePosWorld - transform.position;
        targetDir.z = 0;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.FromToRotation(new Vector3(1, 0, 0), targetDir),
            maxAngleAcc * Time.deltaTime
        );

        if (transform.right.x < 0)
        {
            gunSpriteRenderer.flipY = true;
        }
        else if (transform.right.x > 0)
        {
            gunSpriteRenderer.flipY = false;
        }
        //if ((transform.right.x < 0 && gunSprite.localScale.y > 0)
        //    || (transform.right.x > 0 && gunSprite.localScale.y < 0))
        //{
        //    gunSprite.localScale = new Vector3(
        //        gunSprite.localScale.x, -gunSprite.localScale.y, gunSprite.localScale.z);
        //}
    }
}
