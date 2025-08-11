using System.Globalization;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float speed = 3f;
    Camera cam;

    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        cam = Camera.main; //TODO: Cambiar el sistema de cámara
        rigidbody = this.GetComponent<Rigidbody2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        MovePlayer();
        cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -3); //TODO: Cambiar el sistema de cámara
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector2(horizontal, vertical);

        if (move != Vector3.zero)
        {
            GetComponent<Animator>().SetBool("Runing", true);
            if (horizontal > 0)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;

            move.Normalize();
            move = move * speed;
            rigidbody.linearVelocity = move;
        }
        else
        {
            GetComponent<Animator>().SetBool("Runing", false);
            rigidbody.linearVelocity = Vector3.zero;
        }
    }
}
