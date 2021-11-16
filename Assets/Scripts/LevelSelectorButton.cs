using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelSelectorButton : MonoBehaviour
{
    public void LoadLevel()
    {
        int world;
        int level;
        world = int.Parse(this.gameObject.name.Split(' ')[0]);
        level = int.Parse(this.gameObject.name.Split(' ')[1]);
        PlayerPrefs.SetInt("World", world);
        PlayerPrefs.SetInt("Level", level);
        SceneManager.LoadScene("Game");
    }
}
