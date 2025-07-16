using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterControler : MonoBehaviour
{
    [SerializeField]
    float maxMaskScale = 200;
    [SerializeField]
    float scaleTime = 0.8f;
    [SerializeField]
    float scaleTimeBack = 0.5f;
    [SerializeField]
    Material[] effectMaterials;

    Transform filterMask;
    SpriteRenderer filterSpriteRenderer;

    float scaleTimer;
    // Start is called before the first frame update
    void Start()
    {
        filterMask = transform.GetChild(0);
        foreach (Material mat in effectMaterials)
        {
            mat.SetFloat("_EffectRadius", 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 center = Vector3.zero;
        if (Global.activeTimeTrigger)
        {
            center = Global.activeTimeTrigger.position;
        }
        // float radius = maxMaskScale;
        float radius = scaleTimer * maxMaskScale;
        // update timer
        if (Global.state == STAGE_STATE.TIME_STOPPED)
        {
            scaleTimer = Mathf.Min(scaleTimer + Time.deltaTime / scaleTime, 1);
        }
        else if (Global.state == STAGE_STATE.NORMAL)
        {
            if (Global.areaState == AREA_STATE.RESTARTING)
            {
                scaleTimer = 0;
            }
            else
            {
                scaleTimer = Mathf.Max(scaleTimer - Time.deltaTime / scaleTimeBack, 0);
            }
        }

        // pos
        filterMask.position = center;
        // scale
        filterMask.localScale = new Vector3(radius * 3, radius * 3, 1);
        foreach (Material mat in effectMaterials)
        {
            mat.SetVector("_EffectCenterWorld", center);
            mat.SetFloat("_EffectRadius", radius);
        }
    }
}
