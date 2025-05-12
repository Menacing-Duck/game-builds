using UnityEditor;
using UnityEngine;

public class MainGoblin : MonoBehaviour
{
    public int health;

    public int maxhealth;
    public int damage;

    public GameObject gob;

    public MainGoblin(){
        health = 100;
        maxhealth = 100;
        damage = 10;
    }

    public void gobattack(GameObject target){
        // mettre le systeme de dégats au joueur
    }

    public void gobdeath(){
        Destroy(gob);
    }

    public void gobdamage(int dam){
        health -= dam;
        if(health <= 0){
            gobdeath();
        }
    }

    public void gobheal(int heal){
        health += heal;
        if(health > maxhealth){
            health = maxhealth;
        }
    }


}
