using System.Collections.Generic;
using UnityEngine;

public static class ExperienceSystem
{
    private static List<int> experienceThresholds = new List<int>
    {
        0,      // Lv1
        500,    // Lv2  +500    +500
        1500,   // Lv3  +1000   +500
        3000,   // Lv4  +1500   +500
        5000    // Lv5  +2000   +500
    };

    public static int GetLevel(int experience)
    {
        for (int i = 0; i < experienceThresholds.Count; i++)
            if (experience < experienceThresholds[i])
                return i;
        return experienceThresholds.Count;
    }

    public static ExperienceData GetExperienceData(int experience) //3500
    {
        int level = GetLevel(experience); //4

        if (level >= experienceThresholds.Count)
            return new ExperienceData
            {
                experience = 0,
                experienceNeeded = -1
            }; // Nivel máximo alcanzado
        else
        {
            int currentLevelXP = experienceThresholds[level - 1]; //3000
            int nextLevelXP = experienceThresholds[level]; //5000
            return new ExperienceData
            {
                experience = experience - currentLevelXP, //500
                experienceNeeded = nextLevelXP - currentLevelXP //2000
            };
        }
    }
}

[System.Serializable]
public class ExperienceData
{
    public int experience;
    public int experienceNeeded;

}
