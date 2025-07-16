using NUnit.Framework.Constraints;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    Vector2 borderMin = new Vector2(-35, -20);
    [SerializeField]
    Vector2 borderMax = new Vector2(50, 50);
    [SerializeField]
    float cursorRatio = 0.2f;
    [SerializeField]
    Vector4[] areaBorders;

    float maxPlayerDistance;
    Vector2 cameraCenterOffset;
    Vector2 cameraPosMin;
    Vector2 cameraPosMax;
    Transform player;
    float lastOffset = 0;

    void Start()
    {
        // camera boundary
        float halfHeight = GetComponent<Camera>().orthographicSize;
        float halfWidth = halfHeight * Screen.width / Screen.height;
        cameraCenterOffset = new Vector2(halfWidth, halfHeight);
        cameraPosMin = borderMin + cameraCenterOffset;
        cameraPosMax = borderMax - cameraCenterOffset;
        maxPlayerDistance = cameraCenterOffset.y * 0.8f;

        player = Global.player;
        transform.position = GetValidPosition(player.position);

        lastOffset = 0;
    }

    void Update()
    {
        // area restart, don't move before restart
        if (Global.areaState == AREA_STATE.RESTARTING && !Global.GetAreaResetDone())
        {
            return;
        }

        if (Global.areaId == Global.AREA_MAX)
        {
            lastOffset = Mathf.Clamp((-17f - player.position.x) * 0.9f, 0, 5.0f);
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = player.position * (1 - cursorRatio) + mousePos * cursorRatio;
        targetPos = GetValidPosition(targetPos);
        if (Global.areaState == AREA_STATE.RESTARTING && Global.GetAreaResetDone())
        {
            // immediate set
            transform.position = targetPos;
        }
        else
        {
            Vector3 nextPos = Vector3.MoveTowards(transform.position, targetPos, 20.0f * Time.deltaTime);
            transform.position = nextPos;
        }
    }

    private Vector3 GetValidPosition(Vector3 targetPos)
    {
        // don't move z
        targetPos.z = transform.position.z;

        // distance from player
        // TODO
        Vector2 dir = targetPos - player.position;
        float dist = dir.magnitude;
        if (dist > maxPlayerDistance)
        {
            Vector2 clipDir = dir / dist * maxPlayerDistance;
            targetPos = new Vector3(
                clipDir.x + player.position.x,
                clipDir.y + player.position.y,
                transform.position.z
            );
        }
        // area border
        int areaId = Global.areaId;
        Vector2 areaBorderMin = cameraPosMin;
        Vector2 areaBorderMax = cameraPosMax;
        // TODO
        //if (areaId >= 0 && areaId < areaBorders.Length)
        //{
        //    areaBorderMin = new Vector2(
        //        Mathf.Max(areaBorderMin.x, areaBorders[areaId].x + cameraCenterOffset.x),
        //        Mathf.Max(areaBorderMin.y, areaBorders[areaId].y + cameraCenterOffset.y)
        //    );
        //    areaBorderMax = new Vector2(
        //        Mathf.Min(areaBorderMax.x, areaBorders[areaId].z - cameraCenterOffset.x),
        //        Mathf.Min(areaBorderMax.y, areaBorders[areaId].w - cameraCenterOffset.y)
        //    );
        //}
        // area offset
        //if (areaId >= 0 && areaId < areaBorders.Length)
        //{
        //    areaBorderMin += new Vector2(areaBorders[areaId].x, areaBorders[areaId].y);
        //    areaBorderMax += new Vector2(areaBorders[areaId].z, areaBorders[areaId].w);
        //}

        // special border 1
        if (player.position.x > 10 && player.position.y < 30 && player.position.y > 4)
        {
            areaBorderMin.x += 30;
        }
        // specical border 2
        else if (Global.areaId == Global.AREA_MAX)
        {
            areaBorderMin.x -= lastOffset;
        }

        targetPos.x = Mathf.Clamp(targetPos.x, areaBorderMin.x, areaBorderMax.x);
        targetPos.y = Mathf.Clamp(targetPos.y, areaBorderMin.y, areaBorderMax.y);
        return targetPos;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3[] points = {
            new Vector3(borderMin.x, borderMin.y, 10),
            new Vector3(borderMin.x, borderMax.y, 10),
            new Vector3(borderMax.x, borderMax.y, 10),
            new Vector3(borderMax.x, borderMin.y, 10),
        };
        Gizmos.DrawLineStrip(points, true);

        Gizmos.color = Color.green;
        for (int i = 0; i < areaBorders.Length; i++)
        {
            Gizmos.color = new Color(i * 0.2f, 1, 0);
            Vector2 areaBorderMin = borderMin + new Vector2(areaBorders[i].x, areaBorders[i].y);
            Vector2 areaBorderMax = borderMax + new Vector2(areaBorders[i].z, areaBorders[i].w);
            Vector3[] areaPoints = {
                new Vector3(areaBorderMin.x, areaBorderMin.y, 10),
                new Vector3(areaBorderMin.x, areaBorderMax.y, 10),
                new Vector3(areaBorderMax.x, areaBorderMax.y, 10),
                new Vector3(areaBorderMax.x, areaBorderMin.y, 10),
            };
            Gizmos.DrawLineStrip(areaPoints, true);
        };
    }
}
