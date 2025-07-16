using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletColor : MonoBehaviour
{
    [SerializeField]
    Color baseColorPlayer;
    [SerializeField]
    Color baseColorEnemy;
    [SerializeField]
    Color colorStopped;

    Color baseColor;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        baseColor = transform.parent.CompareTag("PlayerBullet") ?
            baseColorPlayer : baseColorEnemy;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = baseColor;
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.color = Color.Lerp(
            colorStopped, baseColor, Global.GetCountdownLerp());
    }
}
