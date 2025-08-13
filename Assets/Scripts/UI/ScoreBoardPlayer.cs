using UnityEngine;
using TMPro;

public class ScoreBoardPlayer : MonoBehaviour
{
    [SerializeField] TMP_Text txtLevel;
    [SerializeField] TMP_Text txtUsername;
    [SerializeField] TMP_Text txtKills;
    [SerializeField] TMP_Text txtDeaths;
    [SerializeField] TMP_Text txtProportion;

    public void GeneratePlayer(int level, string username, int kills, int deaths, string proportion)
    {
        txtLevel.text = level.ToString();
        txtUsername.text = username;
        txtKills.text = kills.ToString();
        txtDeaths.text = deaths.ToString();
        txtProportion.text = proportion;
    }
}
