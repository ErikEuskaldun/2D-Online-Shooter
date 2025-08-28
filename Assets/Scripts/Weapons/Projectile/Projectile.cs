using Unity.Netcode;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float speed = 5f;
    ulong ownerId;

    public ulong OwnerID => ownerId;
    Rigidbody2D rigidbody;

    void Awake()
    {
        rigidbody = this.GetComponent<Rigidbody2D>();
    }

    public void Setup(ulong ownerId, Material material)
    {
        this.ownerId = ownerId;
        this.GetComponent<SpriteRenderer>().material = material;
    }

    public virtual void Shoot(Vector2 direction)
    {
        rigidbody.linearVelocity = direction * speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        if (collision.collider.gameObject.layer == 7)
        {
            NetPlayer playerHit = collision.collider.GetComponent<NetPlayer>();
            if (playerHit.OwnerClientId == ownerId)
                return;

            playerHit.Hit(damage, ownerId);
        }
    }
}
