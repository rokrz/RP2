using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonVisualUpdate : MonoBehaviour
{
    [SerializeField]
    private Text buttonText;
    public void setTextColorOn()
    {
        buttonText.color = new Color(0.02f,0.89f,0.05f);
    }
    public void setTextColorOff()
    {
        buttonText.color = new Color(0.14f, 0.45f, 0.08f);
    }
}
