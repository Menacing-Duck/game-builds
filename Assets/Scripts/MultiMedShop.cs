using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class Multimedshop : MonoBehaviour
{

    public Stats Player;

    public Stats Playe;

    public GameObject Shop, xp, uplvl, brse, mnabourse;

    public TextMeshProUGUI xpc, upglvl, bourse, manabourse;

        public void Awake()
    {
        Shop = GameObject.FindGameObjectWithTag("Shope");

        xp = GameObject.FindGameObjectWithTag("xpc");

        uplvl = GameObject.FindGameObjectWithTag("upglvl");

        brse = GameObject.FindGameObjectWithTag("bourse");

        mnabourse = GameObject.FindGameObjectWithTag("manabourse");

    }

    public void Start()
    {
        xpc = xp.GetComponent<TextMeshProUGUI>();

        upglvl = uplvl.GetComponent<TextMeshProUGUI>();

        bourse = brse.GetComponent<TextMeshProUGUI>();

        manabourse = mnabourse.GetComponent<TextMeshProUGUI>();

    }


    void Update()
    {
        UpdMana();
        UpdMon();
        
    }

    public void Quit()
    {
        Shop.SetActive(false);
    }


    public void UpdMon(){
        bourse.text = $"Current Money = {Player.Money} coins";
    }

    public void UpdMana(){
        manabourse.text = $"Current Mana = {Player.mana} mana";
    }

    public void Buy10()
    {
        if (Player.Money >= 5 && Player.mana != Player.maxMana)
        {
            if (Player.mana + 10 >= Player.maxMana)
            {
                Player.Mana = Player.MaxMana;
            }
            else
            {
                Player.Mana += 10;
            }
            Player.Money -= 5;
            UpdMana();
            UpdMon();
        }
    }

    public void Buy25(){
        if(Player.Money >= 10 && Player.Mana != Player.MaxMana){
            if(Player.Mana + 25 >= Player.MaxMana){
                Player.Mana = Player.MaxMana;
            }
            else{
                Player.Mana += 25;
            }
            Player.Money -= 10;
            UpdMon();
            UpdMana();
        }
    }

    public void Buy100(){
        if(Player.Money >= 35 && Player.Mana != Player.MaxMana){
            if(Player.Mana + 100 >= Player.MaxMana){
                Player.Mana = Player.MaxMana;
            }
            else{
                Player.Mana += 100;
            }
            Player.Money -= 35;
            UpdMon();
            UpdMana();
        }
    }

    public void buy250(){
        if(Player.Money >= 80 && Player.Mana != Player.MaxMana){
            if(Player.Mana + 250 >= Player.MaxMana){
                Player.Mana = Player.MaxMana;
            }
            else{
                Player.Mana += 250;
            }
            Player.Money -= 80;
            UpdMon();
            UpdMana();
        }
    }

    public void upgstor(){
        
        if(Player.CurLvl == 3){
            if(Player.Money >= 35){
                Player.MaxMana = 250;
                Player.Money -= 35;
                Player.CurLvl = 3;
                xpc.text = "Max level reached!";
                upglvl.text = "4";
                UpdMon();
            }
        }
        if(Player.CurLvl == 2){
            if(Player.Money >= 15){
                Player.MaxMana = 100;
                Player.Money -= 15;
                Player.CurLvl = 3;
                xpc.text = "35 Coins";
                upglvl.text = "3";
                UpdMon();
            }
        }

        if (Player.CurLvl == 1)
        {
            if (Player.Money >= 5)
            {
                Player.MaxMana = 50;
                Player.Money -= 5;
                Player.CurLvl = 2;
                xpc.text = "15 Coins";
                upglvl.text = "2";
                UpdMon();
            }
        }
    }

}