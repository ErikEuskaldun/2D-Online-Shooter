using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        //TODO: Cargarlo de forma asincrona y pantalla de carga...
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
