using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectScreen : MonoBehaviour
{
    public TextMeshProUGUI text, button;

    public bool IsSelected;

    public void select()
    {
        if (IsSelected == false)
        {
            text.text = "Selected";
            button.color = Color.gray;
        }
    }




}
