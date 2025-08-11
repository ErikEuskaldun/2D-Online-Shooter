using UnityEngine;

public class ProjectileBounce : MonoBehaviour
{
    [SerializeField] int bounces = 8;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == 6) //Reduce en uno los rebotes
        {
            bounces--;
            if (bounces < 0)
                Destroy(gameObject);
        }
        //if(collision.collider.gameObject.layer == ??) Combrueba si ha golpeado un jugador y le quita vida
    }
}
