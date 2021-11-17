using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class LevelSelector : MonoBehaviour
{
    public GameObject levelWiki;
    private LevelInfo li;
    public GameObject levelHolder; // panel
    public GameObject levelIcon; //button prefab
    public GameObject thisCanvas;
    public int numberOfLevels = 50; // number of levels
    public Vector2 iconSpacing; 
    private Rect panelDimensions;
    private Rect iconDimensions;
    private int amountPerPage;
    private int currentLevelCount;
    private int originWorld;

    // Start is called before the first frame update
    void Start()
    {
        originWorld = PlayerPrefs.GetInt("MundoLS");
        if (originWorld!=null)
        {
            li = levelWiki.GetComponent<LevelInfo>();
            li.loadLevelsPerWorld();
            numberOfLevels = li.levelsPerWorld[li.worldNames[originWorld]];
            panelDimensions = levelHolder.GetComponent<RectTransform>().rect;
            iconDimensions = levelIcon.GetComponent<RectTransform>().rect;
            int maxInARow = Mathf.FloorToInt((panelDimensions.width + iconSpacing.x) / (iconDimensions.width + iconSpacing.x));
            int maxInACol = Mathf.FloorToInt((panelDimensions.height + iconSpacing.y) / (iconDimensions.height + iconSpacing.y));
            amountPerPage = maxInARow * maxInACol;
            Debug.Log(amountPerPage);
            int totalPages = Mathf.CeilToInt((float)numberOfLevels / amountPerPage);
            LoadPanels(totalPages);
        }
        else
        {
            SceneManager.LoadScene("WorldSelector");
        }
        
    }
    void LoadPanels(int numberOfPanels)
    {
        GameObject panelClone = Instantiate(levelHolder) as GameObject;
        PageSwiper swiper = levelHolder.AddComponent<PageSwiper>();
        swiper.totalPages = numberOfPanels;

        for (int i = 1; i <= numberOfPanels; i++)
        {
            GameObject panel = Instantiate(panelClone) as GameObject;
            panel.transform.SetParent(thisCanvas.transform, false);
            panel.transform.SetParent(levelHolder.transform);
            panel.name = "Page-" + i;
            panel.GetComponent<RectTransform>().localPosition = new Vector2(panelDimensions.width * (i - 1), 0);
            SetUpGrid(panel);
            int numberOfIcons = i == numberOfPanels ? numberOfLevels - currentLevelCount : amountPerPage;
            LoadIcons(numberOfIcons, panel);
        }
        Destroy(panelClone);
    }
    void SetUpGrid(GameObject panel)
    {
        GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(iconDimensions.width, iconDimensions.height);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.spacing = iconSpacing;
    }
    void LoadIcons(int numberOfIcons, GameObject parentObject)
    {
        for (int i = 0; i < numberOfIcons; i++)
        {
            currentLevelCount++;
            GameObject icon = Instantiate(levelIcon) as GameObject;
            icon.transform.SetParent(thisCanvas.transform, false);
            icon.transform.SetParent(parentObject.transform);
            icon.name = originWorld+" "+i;
            icon.GetComponentInChildren<Text>().text=("Level " + currentLevelCount);
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void ReturnToWorlds()
    {
        SceneManager.LoadScene("WorldSelector");
    }

    // Update is called once per frame
    void Update()
    {

    }
}