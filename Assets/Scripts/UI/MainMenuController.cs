using UnityEngine;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] TMP_Text txtVersion;

    private void Awake()
    {
        txtVersion.text = "v" + Application.version;
    }
}
