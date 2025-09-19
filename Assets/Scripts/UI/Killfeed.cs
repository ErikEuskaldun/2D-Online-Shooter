using System.Collections;
using UnityEngine;

public class Killfeed : MonoBehaviour
{
    public static Killfeed Instance;

    [SerializeField] GameObject killfeedPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //StartCoroutine(SpawnRandomKill());
    }

    public void SpawnKillfeedElement(string killer, string victim, int weaponIndex)
    {
        KillfeedPrefab prefab = Instantiate(killfeedPrefab, this.transform).GetComponent<KillfeedPrefab>();
        prefab.transform.SetAsFirstSibling();
        Sprite weaponSprite = WeaponDatabase.Instance.GetWeaponSprite(weaponIndex);
        prefab.SetData(killer, victim, weaponSprite);
    }

    public IEnumerator SpawnRandomKill()
    {
        yield return new WaitForSeconds(2f);
        string[] nombres = { "Victor", "Yoel", "Erik", "Dhoulmagus", "Usuario1", "nombre_usaurio", "dasdsadwqdwq" };
        int intKiller = Random.Range(0, nombres.Length);
        int intVictim = Random.Range(0, nombres.Length);
        int intWeaponIndex = Random.Range(1, 3);
        SpawnKillfeedElement(nombres[intKiller], nombres[intVictim], intWeaponIndex);
        StartCoroutine(SpawnRandomKill());
    }
}
