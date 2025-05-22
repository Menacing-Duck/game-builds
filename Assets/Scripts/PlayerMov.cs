using System;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{
    private float speed = 7f; //c'était à 10 avant mais ça me parait trop rapide bon de tte façon c'est géré direct sur unity
    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveY = 0f;
    private float moveX = 0f;
    public Animator animator;
    public float inputThreshold = 0.1f;

    private bool up = false;
    private bool down = false;
    private bool left = false;
    private bool right = false;
    private bool isWalking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        moveX = 0f;
        moveY = 0f;
        isWalking = false; // On reset ici

        // Gestion des touches
        if (Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", "A"))))
        {
            moveX = -1f;
            left = true;
            isWalking = true;
        }
        if (Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", "D"))))
        {
            moveX = 1f;
            right = true;
            isWalking = true;
        }
        if (Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("up", "W"))))
        {
            moveY = 1f;
            up = true;
            isWalking = true;
        }
        if (Input.GetKey((KeyCode)Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("down", "S"))))
        {
            moveY = -1f;
            down = true;
            isWalking = true;
        }

        // Pour savoir dans quel sens je vais
        Vector2 input = new Vector2(moveX, moveY);

        // Le if est conseillé pour eviter que ce ne soit trop sensible, et apparament il y a un truc avec des tremblements ou jsp quoi
        if (input.magnitude > inputThreshold)
        {
            animator.SetFloat("X", input.x);
            animator.SetFloat("Y", input.y);
            animator.SetFloat("Speed", input.magnitude);
        }
        else
        {
            animator.SetFloat("Speed", 0); // pour interrompre direct l'anim de marche
        }

        animator.SetBool("IsWalking", isWalking);

        // mouvements
        movement = input.normalized;
        rb.linearVelocity = movement * speed;

        Debug.Log($"UP:{up}, DOWN:{down}, LEFT:{left}, RIGHT:{right}, IsWalking:{isWalking}");

        // reset
        up = down = left = right = isWalking = false;
    }
}