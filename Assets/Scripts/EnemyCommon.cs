using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMY_STATE
{
    IDLE,
    ACTIVE,
    STOPPED,
    DEAD
};

public class EnemyCommon : MonoBehaviour
{
    [SerializeField]
    int areaId = 0;
    public int GetAreaId()
    {
        return areaId;
    }
    
//    public ENEMY_STATE GetEnemyState()
//    {
//        // call this manually inside update functions of each types of enemies
//        // to ensure execution order

//    }
}
