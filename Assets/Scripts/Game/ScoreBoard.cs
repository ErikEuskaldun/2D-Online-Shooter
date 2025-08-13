using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    public static ScoreBoard Instance;

    [SerializeField] Transform scoreBoardContainer;
    [SerializeField] Transform popUp;
    [SerializeField] GameObject scoreBoardPlayerPrefab;
    [SerializeField] TMP_Text txtTime;

    bool isEnabled = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //TODO: Hacer todo con suscripciones, de esa forma los datos se actualizan al moemnto.
        // Y si se sale alguen o entra alguien añadir el nuevo usaurio en vez de recargar solo al hacer tab
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            popUp.gameObject.SetActive(true);
            GetAllScores();
            isEnabled = true;
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            popUp.gameObject.SetActive(false);
            isEnabled = false;
        }

        if (isEnabled)
            UpdateTimer();
    }

    public void GetAllScores()
    {
        GetMatchScore();
        GetAllPlayerScore();
    }

    private void UpdateTimer()
    {
        int time = NetworkGameManager.Instance.time.Value;
        int minutes = time / 60;
        int seconds = time % 60;
        txtTime.text = $"{minutes:D2}:{seconds:D2}";
    }

    public void GetMatchScore()
    {
        //TODO: Mostrar la puntuacion de los equipos
    }

    public void GetAllPlayerScore()
    {
        foreach (Transform child in scoreBoardContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            NetworkObject playerNetObject = player.PlayerObject;
            NetPlayer netPlayer = playerNetObject.GetComponent<NetPlayer>();
            GetPlayerScore(netPlayer);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(scoreBoardContainer.GetComponent<RectTransform>());
    }

    public void GetPlayerScore(NetPlayer player)
    {
        FixedString64Bytes user = player.username.Value;
        int kills = player.kills.Value;
        int deaths = player.deaths.Value;
        float proportion = (float)kills / (deaths == 0 ? 1 : deaths);
        string proportionFormatted = proportion.ToString("0.00");

        
        ScoreBoardPlayer scoreBoardPlayer = Instantiate(scoreBoardPlayerPrefab, scoreBoardContainer).GetComponent<ScoreBoardPlayer>();
        scoreBoardPlayer.GeneratePlayer(1, user.ToString(), kills, deaths, proportionFormatted);
    }
}
