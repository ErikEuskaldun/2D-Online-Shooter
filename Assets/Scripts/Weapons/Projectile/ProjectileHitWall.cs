using UnityEngine;

public class ProjectileHitWall : MonoBehaviour
{
    [SerializeField] GameObject VFXPrefav;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 6)
        {
            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, normal);
            Instantiate(VFXPrefav, contact.point, rot);
        }
    }
}
