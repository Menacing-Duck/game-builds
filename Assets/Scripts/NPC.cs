using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{

    public GameObject DialoguePanel;
    public TextMeshProUGUI DialogueText;
    public string[] Dialogue;
    public int Index;

    public GameObject ContButton;

    public float WordSpeed;
    public bool PlayerIsClose;

    void Update()
    {
        KeyCode interactKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("interact", "F"));

        if (Input.GetKeyDown(interactKey) && PlayerIsClose)
        {

            if (DialoguePanel.activeInHierarchy)
            {
                ZeroText();
            }
            else
            {
                DialoguePanel.SetActive(true);
                ContButton.SetActive(false);
                StartCoroutine(Typing());
            }
        }

        if (DialogueText.text == Dialogue[Index])
        {
            ContButton.SetActive(true);
        }
    
    
}

    public void ZeroText(){
        DialogueText.text = "";
    }

    IEnumerator Typing(){
        foreach(char letter in Dialogue[Index].ToCharArray()){
            DialogueText.text += letter;
            yield return new WaitForSeconds(WordSpeed);
        }
    }

    public void NextLine(){
        ContButton.SetActive(false);

        if (Index < Dialogue.Length - 1)
        {
            Index++;
            DialogueText.text = "";
            ZeroText();
            StartCoroutine(Typing());
        }
        else
        {
            ZeroText();
            DialoguePanel.SetActive(false);
            Index = 0;
        }
    }

private void OnTriggerEnter2D(Collider2D other)
{
    Debug.Log("OnTriggerEnter2D called with: " + other.gameObject.name);

    if (other.CompareTag("Player"))
    {
        ZeroText();
        Debug.Log("Player entered the trigger!");
        PlayerIsClose = true;
    }
}

private void OnTriggerExit2D(Collider2D other)
{
    Debug.Log("OnTriggerExit2D called with: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the trigger!");
            PlayerIsClose = false;
            StopAllCoroutines();
            ZeroText();
            DialoguePanel.SetActive(false);
            Index = 0;
    }
}

}
