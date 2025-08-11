using System.IO;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;
    private PlayerData data;

    public string Username => data.username;
    public int Level => ExperienceSystem.GetLevel(data.xp);
    public ExperienceData Experience => ExperienceSystem.GetExperienceData(data.xp);

    private string savePath => Path.Combine(Application.persistentDataPath, "SaveData", "playerdata.save");
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadPlayerData();
    }

    public void LoadPlayerData()
    {
        if (!File.Exists(savePath))
        {
            string dirPath = Path.Combine(Application.persistentDataPath, "SaveData");
            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);
            data = new PlayerData
            {
                username = "Guest" + Random.Range(0, 999999),
                xp = 0
            };
            Save();
        }
        else
            Load();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
    }

    public void Load()
    {
        string json = File.ReadAllText(savePath);
        data = JsonUtility.FromJson<PlayerData>(json);
    }

    public void SetUsername(string username)
    {
        data.username = username;
    }

    [ContextMenu("Give 100XP")]
    public void Give100XP()
    {
        GiveXP(100);
        Save();
    }

    public void GiveXP(int xp)
    {
        int startingLevel = Level;
        data.xp += xp;
        int level = Level;
        if(level > startingLevel)
        {
            //TODO: Level Up
            Debug.Log("Level Up ==> " + level);
        }
        ExperienceData xpData = ExperienceSystem.GetExperienceData(data.xp);
        float percent = (float)xpData.experience / xpData.experienceNeeded;
        Debug.Log("Progress Bar: " + level + " ==[" + xpData.experience + "]== (" + percent + ") ==[" + xpData.experienceNeeded + "]== " + (level + 1));
    }

}

[System.Serializable]
public class PlayerData
{
    public string username;
    public int xp;
}
