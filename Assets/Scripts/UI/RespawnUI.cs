using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class RespawnUI : MonoBehaviour
{
    [SerializeField] private TMP_Text txtSpawning;
    [SerializeField] private Material spriteMaterial;

    public static RespawnUI Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SetActive(bool active)
    {
        float gradient = active ? 0.5f : 1f;
        spriteMaterial.SetFloat("_GradientAdjustment", gradient);
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
        spriteMaterial.SetFloat("_GradientAdjustment", 1f);
    }
}
