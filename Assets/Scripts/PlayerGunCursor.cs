using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunCursor : MonoBehaviour
{
    [SerializeField]
    Texture2D cursorAimTexture;

    [SerializeField]
    Texture2D[] cursorChargeTexture;

    [SerializeField]
    Texture2D cursorDisableTexture;

    [SerializeField]
    Vector2 cursorHotspot = new Vector2(16, 16);

    PlayerGunShoot shoot;

    // Start is called before the first frame update
    void Start()
    {
        shoot = GetComponent<PlayerGunShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Global.GetEndingTriggered())
        {
            // reset cursor on game end
            Cursor.SetCursor(cursorAimTexture, cursorHotspot, CursorMode.Auto);
            return;
        }
        if (shoot.GetShootDisabled())
        {
            Cursor.SetCursor(cursorDisableTexture, cursorHotspot, CursorMode.Auto);
            return;
        }
        //float charge = shoot.GetChargeRate();
        int levels = cursorChargeTexture.Length;
        int charge = Mathf.FloorToInt(shoot.GetChargeRate() * levels);
        if (charge <= 0)
        {
            Cursor.SetCursor(cursorAimTexture, cursorHotspot, CursorMode.Auto);
            return;
        }
        Cursor.SetCursor(cursorChargeTexture[charge - 1], cursorHotspot, CursorMode.Auto);
    }
}
