using TMPro;
using UnityEngine;

public class PlayerDataUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ifUsername;
    [SerializeField] private TMP_Text txtLevel;
    [SerializeField] private RectTransform xpBar;

    private void Awake()
    {
        ifUsername.onEndEdit.AddListener(ChangeUsername);
    }

    private void Start()
    {
        ifUsername.text = PlayerDataManager.Instance.Username;
        SetLevel();
    }

    private void SetLevel()
    {
        txtLevel.text = PlayerDataManager.Instance.Level.ToString();
        ExperienceData experienceData = PlayerDataManager.Instance.Experience;
        float percent = (float)experienceData.experience / experienceData.experienceNeeded;
        xpBar.localScale = new Vector3(percent, xpBar.localScale.y, xpBar.localScale.z);
    }

    private void ChangeUsername(string username)
    {
        PlayerDataManager.Instance.SetUsername(username);
        PlayerDataManager.Instance.Save();
    }
}
