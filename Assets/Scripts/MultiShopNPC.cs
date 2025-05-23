using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MultiShopNPC : MonoBehaviour
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
        KeyCode interactKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", "F"));

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

        if (other.CompareTag("Shop"))
        {
            PlayerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Shop"))
        {
            PlayerIsClose = false;
        }

    }
}