using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSelectorButton : MonoBehaviour
{
    string worldName;
    public void LoadWorldLevels()
    {
        this.worldName = this.gameObject.name;
        PlayerPrefs.SetInt("MundoLS", int.Parse(worldName));
        SceneManager.LoadScene("LevelSelector");
    }
}
