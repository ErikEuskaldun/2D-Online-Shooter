using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    [SerializeField] Texture2D cursorCrosshair;
    [SerializeField] Texture2D cursorHit;

    private Coroutine hitmarkerCorutine;
    private const float HITMARKER_MAX_TIME = 0.15f;

    private void Awake()
    {
        Instance = this;
        SetCroshair();
    }

    public void SetCroshair()
    {
        Vector2 cursorHotspot = new Vector2(cursorCrosshair.width / 2, cursorCrosshair.height / 2);
        Cursor.SetCursor(cursorCrosshair, cursorHotspot, CursorMode.Auto);
    }

    public void Hit()
    {
        if(hitmarkerCorutine != null)
            StopCoroutine(hitmarkerCorutine);

        hitmarkerCorutine = StartCoroutine(HitMarkerAsync());
    }

    public IEnumerator HitMarkerAsync()
    {
        Vector2 cursorHotspot = new Vector2(cursorHit.width / 2, cursorHit.height / 2);
        Cursor.SetCursor(cursorHit, cursorHotspot, CursorMode.Auto);

        yield return new WaitForSeconds(HITMARKER_MAX_TIME);

        SetCroshair();
        hitmarkerCorutine = null;
    }
}
