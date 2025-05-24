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

    public void Awake()
    {
        Shop = GameObject.FindGameObjectWithTag("Shope");
        if (Shop == null) Debug.LogWarning("Shop non trouvé avec le tag 'Shop'");

        MedShop = GameObject.FindGameObjectWithTag("MedShop");
        if (MedShop == null) Debug.LogWarning("MedShop non trouvé avec le tag 'MedShop'");

        ModShop = GameObject.FindGameObjectWithTag("ModShop");
        if (ModShop == null) Debug.LogWarning("ModShop non trouvé avec le tag 'ModShop'");
    }

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
        Debug.Log("collision !!");

        if (other.CompareTag("Shop"))
        {
            PlayerIsClose = true;
                    Debug.Log("shop détécté");

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {

        if (other.CompareTag("Shop"))
        {
            PlayerIsClose = false;
                    Debug.Log("quitte la range du shop");
        }
                Debug.Log("plus de collision");
    }
}