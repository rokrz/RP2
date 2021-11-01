using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelButton;
    int currentPage = 0;
    private Dictionary<string, List<LevelCreator>> worldHolder;
    private string[] worlds;

    private void Start()
    {
        worldHolder = GridMaker.instance.worldHolder;
        worlds = new string[worldHolder.Keys.Count];
        worldHolder.Keys.CopyTo(worlds,0);
        LoadLevelsPerPage();
    }

    void NextPage()
    {
        currentPage++;
        LoadLevelsPerPage();
    }

    void PreviousPage() {
        currentPage--;
        LoadLevelsPerPage();
    }

    void LoadLevelsPerPage()
    {
        int level = 0;
        if (currentPage>=0 && currentPage<worlds.Length)
        {
            foreach (LevelCreator lc in worldHolder[worlds[currentPage]])
            {
                GameObject auxObject = Instantiate(levelButton, new Vector3(0.0f, 0.0f), Quaternion.identity);
                auxObject.GetComponent<LevelSelectorButton>().SetButtonValues(currentPage, level);
                level++;
            }
        }
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}
