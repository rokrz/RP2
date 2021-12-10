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

    public GameObject PageControl; //button prefab
    public GameObject UIPanel; // panel
    public PageSwiper swiper;


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
            if (totalPages > 1)
            {
                GameObject icontrue = Instantiate(PageControl) as GameObject;
                icontrue.transform.SetParent(thisCanvas.transform, false);
                icontrue.transform.SetParent(UIPanel.transform);
                icontrue.name = "true";
                icontrue.GetComponentInChildren<Text>().text = ("Next");
                icontrue.transform.position = new Vector3(920, 90, 0);
                //icontrue.GetComponent<PageControl>().setPG(swiper);
                GameObject.Instantiate(icontrue);


                GameObject iconfalse = Instantiate(PageControl) as GameObject;
                iconfalse.transform.SetParent(thisCanvas.transform, false);
                iconfalse.transform.SetParent(UIPanel.transform);
                iconfalse.name = "false";
                iconfalse.GetComponentInChildren<Text>().text = ("Prev");
                iconfalse.transform.position = new Vector3(170, 90, 0);
                //iconfalse.GetComponent<PageControl>().setPG(swiper);
                GameObject.Instantiate(iconfalse);

            }

            

        }
        else
        {
            SceneManager.LoadScene("WorldSelector");
        }
        
    }
    void LoadPanels(int numberOfPanels)
    {
        GameObject panelClone = Instantiate(levelHolder) as GameObject;
        swiper = levelHolder.AddComponent<PageSwiper>();
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