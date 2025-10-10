using Unity.Netcode;
using UnityEngine;

public class ProjectileDieOnDistance : MonoBehaviour
{
    [SerializeField] float maxDistance = 10f;
    private Vector2 startingPosition;

    private void Awake()
    {
        startingPosition = transform.position;
    }

    public void SetDistance(float distance)
    {
        maxDistance = distance;
    }

    private void Update()
    {
        float distance = Vector2.Distance(startingPosition, transform.position);

        if(distance > maxDistance)
            Destroy(this.gameObject);
    }
}
