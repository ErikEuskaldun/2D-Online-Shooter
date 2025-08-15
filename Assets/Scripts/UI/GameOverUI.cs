using UnityEngine;
using TMPro;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] GameObject popUp;
    [SerializeField] TMP_Text txtWinner;
    [SerializeField] TMP_Text txtLobbyCountdwn;

    public static GameOverUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PopUp(string winner, int seconds)
    {
        popUp.gameObject.SetActive(true);
        txtWinner.text = winner;
        StartCoroutine(LobbyCountDown(seconds));
    }
    public void PopOut()
    {
        popUp.gameObject.SetActive(false);
    }

    private IEnumerator LobbyCountDown(int seconds)
    {
        do
        {
            txtLobbyCountdwn.text = "Next match in " + seconds;
            yield return new WaitForSeconds(1);
            seconds--;
        } while (seconds > 0);
    }
}
