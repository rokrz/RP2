using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text currentLevelText;
    // Start is called before the first frame update
    void Start()
    {
        currentLevelText.text = "Mundo: " + (PlayerPrefs.GetInt("World") + 1)+"\nNível: "+(PlayerPrefs.GetInt("Level")+1).ToString();
    }

    public void updateCurrentLevelText()
    {
        currentLevelText.text = "Mundo: " + (PlayerPrefs.GetInt("World") + 1)+ "\nNível: " + (PlayerPrefs.GetInt("Level") + 1).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
