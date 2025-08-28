using UnityEngine;

public class LoadoutGameMenu : MonoBehaviour
{
    public static LoadoutGameMenu Instance;

    [SerializeField] GameObject LoadoutsMenu;

    private void Awake()
    {
        Instance = this;
    }

    public void SetVisible(bool isVisible = true)
    {
        LoadoutsMenu.SetActive(isVisible);
    }
}
