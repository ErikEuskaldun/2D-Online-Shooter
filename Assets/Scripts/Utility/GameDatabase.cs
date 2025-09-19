using System.Collections.Generic;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class GameDatabase : MonoBehaviour
{
    [SerializeField] Material allyTeamMaterial;
    [SerializeField] Material enemyTeamMaterial;
    [SerializeField] Material allySpriteMaterial;
    [SerializeField] Material enemySpriteMaterial;

    public static GameDatabase Instance;
    private void Awake()
    {
        if(Instance != null )
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    public Material GetPlayerMaterial(bool isAlly)
    {
        return isAlly ? allyTeamMaterial : enemyTeamMaterial;
    }

    public Material GetSpriteMaterial(bool isAlly)
    {
        return isAlly ? allySpriteMaterial : enemySpriteMaterial;
    }

    public List<Material> GetAllMaterial()
    {
        List<Material> result = new List<Material>();
        result.Add(allyTeamMaterial);
        result.Add(enemyTeamMaterial);
        result.Add(allySpriteMaterial); 
        result.Add(enemySpriteMaterial);
        return result;
    }
}
