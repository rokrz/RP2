using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelectorButton : MonoBehaviour
{
    public int Mundo;
    public int Level;

    public void SetButtonValues(int mundo, int level)
    {
        this.Mundo = mundo;
        this.Level = level;
    }

    public void LoadLevel(int world, int level)
    {
        PlayerPrefs.SetInt("World", world);
        PlayerPrefs.SetInt("Level", level);
        SceneManager.LoadScene("Game");
    }
}
