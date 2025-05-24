using System;
using UnityEngine;
using Unity.Netcode;
public class PlayerMovement : NetworkBehaviour
{
    private float speed = 7f; //c'était à 10 avant mais ça me parait trop rapide bon de tte façon c'est géré direct sur unity
    private Rigidbody2D rb;
    private Vector2 movement;
    private float moveY = 0f;
    private float moveX = 0f;
    public Animator animator; // Normalement pas besoin de mettre en public grâce au void awake
    public float inputThreshold = 0.1f;

    private bool up = false;
    private bool down = false;
    private bool left = false;
    private bool right = false;
    private bool isWalking = false;


    //Ici je met des variables pour actualiser TOUS les mouvements des différents joueurs online
    private NetworkVariable<Vector2> netAnimDirection = new NetworkVariable<Vector2>(writePerm: NetworkVariableWritePermission.Owner);

    private NetworkVariable<float> netSpeed = new NetworkVariable<float>(writePerm: NetworkVariableWritePermission.Owner);

    private NetworkVariable<bool> netIsWalking = new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);

    private Vector2 lastDirection = Vector2.down; // Pour sauvegarder la dernière direction dans laquelle est allé le perso, et ainsi activer la bonne animation Idle


    public override void OnNetworkSpawn() //Il vient après Awake, au moment de la connexion d'un joueur, il initialise ses droits etc...
    {
        if (!IsOwner) return;
        rb = GetComponent<Rigidbody2D>();
    }

    void Awake() //Il initialise tout avant tout, en gros il permet de régler un problème, on pouvait pas glisser l'animator dans le game object, du coup ici il evite qu'on ai à le faire
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator is NULL!"); //au cas où il trouve pas l'animator
        }

    }


    void Update()
    {
        if (!IsOwner) return;
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

        if (input.magnitude > inputThreshold)
        {
            lastDirection = input;// On met à jour localement la dernière position ??utile?? --> je crois pas finalement
            netAnimDirection.Value = input; //On met à jour la valeur de direction comme normalement
            netSpeed.Value = input.magnitude;
        }
        else
        {
            netAnimDirection.Value = lastDirection; // On transmet la dernière direction ici aussi !
            netSpeed.Value = 0;
        }

        netIsWalking.Value = isWalking;


        //netAnimDirection.Value = new Vector2(moveX, moveY);
        // --> cette ligne prend la valeur local mais normalement il n'y en a pas besoin avec ce qu'il y a juste au dessus

        // Le if est conseillé pour eviter que ce ne soit trop sensible, et apparament il y a un truc avec des tremblements ou jsp quoi
        if (input.magnitude > inputThreshold)
        {
            animator.SetFloat("X", input.x);
            animator.SetFloat("Y", input.y);
        }
        else
        {
            animator.SetFloat("X", lastDirection.x); //Ici on reprend la dernière direction quand il a arrêté de bouger
            animator.SetFloat("Y", lastDirection.y);
        }

        animator.SetBool("IsWalking", isWalking);

        // mouvements
        movement = input.normalized;
        rb.linearVelocity = movement * speed;

        // Debug.Log($"UP:{up}, DOWN:{down}, LEFT:{left}, RIGHT:{right}, IsWalking:{isWalking}");

        // reset
        up = down = left = right = isWalking = false;
    }
    
    void FixedUpdate() //pour update les animations sur le networking
    {
        Vector2 direction = netAnimDirection.Value;
        float speed = netSpeed.Value;
        bool walking = netIsWalking.Value;

        animator.SetFloat("X", direction.x);
        animator.SetFloat("Y", direction.y);
        animator.SetBool("IsWalking", walking);
    }   



}


