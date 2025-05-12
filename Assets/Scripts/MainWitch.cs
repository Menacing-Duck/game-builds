using UnityEngine;

public class MainWitch : MonoBehaviour
{
    public int health;

    public int maxhealth;

    public int damage;

    public GameObject witch;

    public MainWitch(){
        health = 250;
        maxhealth = 250;
        damage = 25;
    }

    public void wicdeath(){
        Destroy(witch);
    }

    public void wicattack(GameObject target)
    {
        // implémenter la vie du joueur
    }

    public void wicheal(int heal){
        health += heal;
        if(health > maxhealth){
            health = maxhealth;
        }
        
    }

    public void wicdamage(int dam)
    {
        health -= dam;
        if(health <= 0){
            wicdeath();
        }
    }


}
