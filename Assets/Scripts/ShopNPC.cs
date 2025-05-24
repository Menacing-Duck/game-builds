using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopNPC : MonoBehaviour
{



    public GameObject Shop;

    public GameObject MedShop;

    public GameObject ModShop;

    public PlayerEco Player;


    public bool PlayerIsClose;

    void Start()
    {
        if (Player.IsMed)
        {
            MedShop.SetActive(true);
        }
        else
        {
            ModShop.SetActive(true);
        }
    }

    void Update()
    {
        KeyCode interactKey = KeyCode.Space;

        if (Input.GetKeyDown(interactKey) && PlayerIsClose)
        {
            if (Shop.activeInHierarchy)
            {
                Shop.SetActive(false);
            }
            else
            {
                Shop.SetActive(true);
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            PlayerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            PlayerIsClose = false;

        }

    }
}