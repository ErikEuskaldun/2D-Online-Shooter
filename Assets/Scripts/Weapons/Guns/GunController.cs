using Unity.VisualScripting;
using UnityEngine;

public class GunController : MonoBehaviour
{
    [SerializeField] GunScriptable testGun;
    void Start()
    {
        Instantiate(testGun.prefab, this.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
