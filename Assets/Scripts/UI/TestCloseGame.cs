using UnityEngine;

public class TestCloseGame : MonoBehaviour
{
    [SerializeField] GameObject closeGameGameObject;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            closeGameGameObject.SetActive(!closeGameGameObject.activeSelf);
    }
}
