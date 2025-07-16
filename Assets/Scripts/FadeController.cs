using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    [SerializeField]
    Color colorReset;
    [SerializeField]
    Color colorEnd;

    SpriteRenderer fadeSpriteRenderer;
    SpriteRenderer endingTextSpriteRenderer;

    private void Start()
    {
        fadeSpriteRenderer = transform.GetChild(0)
            .gameObject.GetComponent<SpriteRenderer>();
        endingTextSpriteRenderer = transform.GetChild(1)
            .gameObject.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        bool fadeEnding = Global.GetEndingTriggered();
        Color fadeColor = fadeEnding ? colorEnd : colorReset;
        float t = fadeEnding ? Global.GetEndingLerp() : Global.GetAreaRestartLerp();
        fadeColor.a = t;
        fadeSpriteRenderer.color = fadeColor;
        if (fadeEnding)
        {
            float tText = 1 - (1 - t) * (1 - t);
            endingTextSpriteRenderer.color = new Color(1, 1, 1, tText);
        }
    }
}
