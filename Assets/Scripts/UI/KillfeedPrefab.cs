using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class KillfeedPrefab : MonoBehaviour
{
    [SerializeField] TMP_Text txtNameKiller;
    [SerializeField] TMP_Text txtNameVictim;
    [SerializeField] Image imgWeapon;

    public void SetData(string killer, string victim, Sprite weapon)
    {
        txtNameKiller.text = killer;
        txtNameVictim.text = victim;
        imgWeapon.sprite = weapon;

        StartCoroutine(DieAsync());

        LayoutRebuilder.ForceRebuildLayoutImmediate( (RectTransform)transform );

        //TODO: Si la lista de childs es mayor a X quitar el ultimo (de esta forma tiene un máximo de X)
    }

    public IEnumerator DieAsync()
    {
        yield return new WaitForSeconds(8f);
        Destroy(this.gameObject);
    }
}
