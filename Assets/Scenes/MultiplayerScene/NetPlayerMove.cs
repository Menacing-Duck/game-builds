using System;
using UnityEngine;
using Unity.Netcode;
public class PlayerMovement : NetworkBehaviour
{
    public float speed = 10f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveX = 0f;
    private float moveY = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if(!IsOwner) return;
        moveX = 0f;
        moveY = 0f;


        if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("left", "A"))))
            moveX = -1f; // Gauche
        if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("right", "D")))) 
            moveX = 1f;  // Droite
        if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("up", "W")))) 
            moveY = 1f;  // Haut
        if (Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("down", "S")))) 
            moveY = -1f; // Bas

        movement = new Vector2(moveX, moveY).normalized;

        rb.linearVelocity = movement * speed;
    }
}


