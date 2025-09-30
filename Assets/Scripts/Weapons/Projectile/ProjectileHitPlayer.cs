using UnityEngine;

public class ProjectileHitPlayer: MonoBehaviour
{
    [SerializeField] GameObject VFXPrefav;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 7)
        {
            //Es el usuario del arma
            NetPlayer playerHit = collision.collider.GetComponent<NetPlayer>();
            if (playerHit.OwnerClientId == GetComponent<Projectile>().OwnerID)
                return;

            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, normal);
            Instantiate(VFXPrefav, contact.point, rot);
        }
    }
}
