using System.Collections;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    [SerializeField] float seconds;

    private void Awake()
    {
        StartCoroutine(DestroyAsync());
    }

    private IEnumerator DestroyAsync()
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
}
