using UnityEngine;
using UnityEngine.Rendering.Universal;

public class test : MonoBehaviour
{
    [SerializeField] PixelPerfectCamera pixelPrefectCamera;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            if (Input.GetKeyDown(KeyCode.P))
                pixelPrefectCamera.enabled = !pixelPrefectCamera.enabled;
    }
}
