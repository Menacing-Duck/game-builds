using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class medshop : MonoBehaviour
{

    public PlayerEco Player;

    public GameObject Shop;

    public TextMeshProUGUI xpc, upglvl, bourse, quit, manabourse;

    public void Quit(){
        Shop.SetActive(false);
    }


    public void UpdMon(){
        bourse.text = $"Current Money = {Player.Money} coins";
    }

    public void UpdMana(){
        manabourse.text = $"Current Mana = {Player.Money} mana";
    }

    public void Buy10(){
        if(Player.Money >= 5){
            if(Player.Mana + 10 >= Player.MaxMana){
                Player.Mana = Player.MaxMana;
            }
            else{
                Player.Mana += 10;
            }
            Player.Money -= 5;
            UpdMana();
            UpdMon();
        }
    }

    public void Buy25(){
        if(Player.Money >= 10){
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
        if(Player.Money >= 35){
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
        if(Player.Money >= 80){
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
        if(Player.CurLvl == 1){
            if(Player.Money >= 5){
                Player.MaxMana = 50;
                Player.Money -= 5;
                Player.CurLvl = 2;
                xpc.text = "15 Coins";
                upglvl.text = "2";
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

    
    }

}
