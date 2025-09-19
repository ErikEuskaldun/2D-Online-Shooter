using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Collections.Generic;

public class RespawnUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtSpawning;

    public static RespawnUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetActive(bool active)
    {
        float gradient = active ? 0.5f : 1f;
        SetAllMaterialGradient(gradient);
        txtSpawning.enabled = active;
    }

    public void SetCounter(int second)
    {
        StartCoroutine(Counter(second));
    }

    private IEnumerator Counter(int seconds)
    {
        LoadoutGameMenu.Instance.SetVisible(true);
        int timer = seconds;
        do
        {
            txtSpawning.text = "Spawning in " + timer.ToString();
            yield return new WaitForSeconds(1);
            timer--;
        } while (timer > 0);
        LoadoutGameMenu.Instance.SetVisible(false);
    }

    private void OnDestroy()
    {
        SetAllMaterialGradient(1f);
    }

    private void SetAllMaterialGradient(float gradient)
    {
        List<Material> materialList = GameDatabase.Instance.GetAllMaterial();
        foreach (Material mat in materialList)
        {
            if (mat.name == "SpriteEnemy") //las balas deben quedarse rojas
                continue;
            mat.SetFloat("_GradientAdjustment", gradient);
        }
        
    }
}
