using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] protected GunScriptable gunInfo;
    protected int currentAmmo;
    [Header("Projectile")]
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] protected Transform projectileSpawnPosition;

    private float shootTimer;

    private void Start()
    {
        currentAmmo = gunInfo.ammo;
    }

    private void Update()
    {
        Rotate();
        ShotController();
    }

    void Rotate()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector3 direction = (mousePosition - transform.position);
        float angle = Mathf.Atan2 (direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void ShotController()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer > 0f)
            return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Shot();
            shootTimer = gunInfo.cadence;
        } 
    }

    public virtual void Shot()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 direction = (mousePosition - projectileSpawnPosition.position).normalized;
        Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPosition.position, Quaternion.identity).GetComponent<Projectile>();
        projectile.Shoot(direction.normalized);
    }
}
