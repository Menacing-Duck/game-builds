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
        Shop = GameObject.FindGameObjectWithTag("Shope2");

        xp = GameObject.FindGameObjectWithTag("xpc2");

        uplvl = GameObject.FindGameObjectWithTag("upglvl2");

        brse = GameObject.FindGameObjectWithTag("bourse2");

        mnabourse = GameObject.FindGameObjectWithTag("manabourse2");

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
        manabourse.text = $"Current Mana = {Player.Mana} mana";
    }

    public void Buy10()
    {
        if (Player.Money >= 5 && Player.Mana != Player.MaxMana)
        {
            if (Player.Mana.Value + 10 >= Player.MaxMana.Value)
            {
                Player.Mana.Value = Player.MaxMana.Value;
            }
            else
            {
                Player.Mana.Value += 10;
            }
            Player.Money -= 5;
            UpdMana();
            UpdMon();
        }
    }

    public void Buy25(){
        if(Player.Money >= 10 && Player.Mana != Player.MaxMana){
            if(Player.Mana.Value + 25 >= Player.MaxMana.Value){
                Player.Mana = Player.MaxMana;
            }
            else{
                Player.Mana.Value += 25;
            }
            Player.Money -= 10;
            UpdMon();
            UpdMana();
        }
    }

    public void Buy100(){
        if(Player.Money >= 35 && Player.Mana != Player.MaxMana){
            if(Player.Mana.Value + 100 >= Player.MaxMana.Value){
                Player.Mana = Player.MaxMana;
            }
            else{
                Player.Mana.Value += 100;
            }
            Player.Money -= 35;
            UpdMon();
            UpdMana();
        }
    }

    public void buy250(){
        if(Player.Money >= 80 && Player.Mana != Player.MaxMana){
            if(Player.Mana.Value + 250 >= Player.MaxMana.Value){
                Player.Mana = Player.MaxMana;
            }
            else{
                Player.Mana.Value += 250;
            }
            Player.Money -= 80;
            UpdMon();
            UpdMana();
        }
    }

    public void upgstor(){
        
        if(Player.CurLvl.Value == 3){
            if(Player.Money >= 35){
                Player.MaxMana.Value = 250;
                Player.Money -= 35;
                Player.CurLvl.Value = 3;
                xpc.text = "Max level reached!";
                upglvl.text = "4";
                UpdMon();
            }
        }
        if(Player.CurLvl.Value == 2){
            if(Player.Money >= 15){
                Player.MaxMana.Value = 100;
                Player.Money -= 15;
                Player.CurLvl.Value = 3;
                xpc.text = "35 Coins";
                upglvl.text = "3";
                UpdMon();
            }
        }

        if (Player.CurLvl.Value == 1)
        {
            if (Player.Money >= 5)
            {
                Player.MaxMana.Value = 50;
                Player.Money -= 5;
                Player.CurLvl.Value = 2;
                xpc.text = "15 Coins";
                upglvl.text = "2";
                UpdMon();
            }
        }
    }

}