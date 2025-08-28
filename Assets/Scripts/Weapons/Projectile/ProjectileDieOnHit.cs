using Unity.Netcode;
using UnityEngine;

public class ProjectileDieOnHit : MonoBehaviour
{
    [SerializeField] LayerMask layers;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & layers) != 0)
        {
            if (collision.gameObject.layer == 7) //Player
                if (GetComponent<Projectile>().OwnerID == collision.gameObject.GetComponent<NetworkObject>().OwnerClientId)
                    return;
            Destroy(this.gameObject);
        }
    }
}
